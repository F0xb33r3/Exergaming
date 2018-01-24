/*
This is the base adapter from which all the other adapters extend.
*/

class Adapter{

  constructor(el, block, options){
    this.el = el;
    this.block = block;
    this.options = options;
    this.pressed = false;
    this.deepPressed = false;
    this.nativeSupport = false;
    this.runningPolyfill = false;
    this.runKey = Math.random();
  }

  setPressed(boolean){
    this.pressed = boolean;
  }

  setDeepPressed(boolean){
    this.deepPressed = boolean;
  }

  isPressed(){
    return this.pressed;
  }

  isDeepPressed(){
    return this.deepPressed;
  }

  add(event, set){
    this.el.addEventListener(event, set, false);
  }

  runClosure(method){
    if(method in this.block){
      // call the closure method and apply nth arguments if they exist
      this.block[method].apply(this.el, Array.prototype.slice.call(arguments, 1));
    }
  }

  fail(event, runKey){
    if(Config.get('polyfill', this.options)){
      if(this.runKey === runKey){
        this.runPolyfill(event);
      }
    } else {
      this.runClosure('unsupported', event);
    }
  }

  bindUnsupportedEvent(){
    this.add(supportsTouch ? 'touchstart' : 'mousedown', (event) => this.runClosure('unsupported', event));
  }

  _startPress(event){
    if(this.isPressed() === false){
      this.runningPolyfill = false;
      this.setPressed(true);
      this.runClosure('start', event);
    }
  }

  _startDeepPress(event){
    if(this.isPressed() && this.isDeepPressed() === false){
      this.setDeepPressed(true);
      this.runClosure('startDeepPress', event);
    }
  }

  _changePress(force, event){
    this.nativeSupport = true;
    this.runClosure('change', force, event);
  }

  _endDeepPress(){
    if(this.isPressed() && this.isDeepPressed()){
      this.setDeepPressed(false);
      this.runClosure('endDeepPress');
    }
  }

  _endPress(){
    if(this.runningPolyfill === false){
      if(this.isPressed()){
        this._endDeepPress();
        this.setPressed(false);
        this.runClosure('end');
      }
      this.runKey = Math.random();
      this.nativeSupport = false;
    } else {
      this.setPressed(false);
    }
  }

  deepPress(force, event){
    force >= 0.5 ? this._startDeepPress(event) : this._endDeepPress();
  }

  runPolyfill(event){
    this.increment = Config.get('polyfillSpeedUp', this.options) === 0 ? 1 : 10 / Config.get('polyfillSpeedUp', this.options);
    this.decrement = Config.get('polyfillSpeedDown', this.options) === 0 ? 1 : 10 / Config.get('polyfillSpeedDown', this.options);
    this.setPressed(true);
    this.runClosure('start', event);
    if(this.runningPolyfill === false){
      this.loopPolyfillForce(0, event);
    }
  }

  loopPolyfillForce(force, event){
    if(this.nativeSupport === false){
      if(this.isPressed()) {
        this.runningPolyfill = true;
        force = force + this.increment > 1 ? 1 : force + this.increment;
        this.runClosure('change', force, event);
        this.deepPress(force, event);
        setTimeout(this.loopPolyfillForce.bind(this, force, event), 10);
      } else {
        force = force - this.decrement < 0 ? 0 : force - this.decrement;
        if(force < 0.5 && this.isDeepPressed()){
          this.setDeepPressed(false);
          this.runClosure('endDeepPress');
        }
        if(force === 0){
          this.runningPolyfill = false;
          this.setPressed(true);
          this._endPress();
        } else {
          this.runClosure('change', force, event);
          this.deepPress(force, event);
          setTimeout(this.loopPolyfillForce.bind(this, force, event), 10);
        }
      }
    }
  }

}


/*
This adapter is more mobile devices that support 3D Touch.
*/

class Adapter3DTouch extends Adapter{

  constructor(el, block, options){
    super(el, block, options);
  }

  bindEvents(){
    if(supportsTouchForceChange){
      this.add('touchforcechange', this.start.bind(this));
      this.add('touchstart', this.support.bind(this, 0));
      this.add('touchend', this._endPress.bind(this));
    } else {
      this.add('touchstart', this.startLegacy.bind(this));
      this.add('touchend', this._endPress.bind(this));
    }
  }

  start(event){
    if(event.touches.length > 0){
      this._startPress(event);
      this.touch = this.selectTouch(event);
      if (this.touch) {
        this._changePress(this.touch.force, event);
      }
    }
  }

  support(iter, event, runKey = this.runKey){
    if(this.isPressed() === false){
      if(iter <= 6){
        iter++;
        setTimeout(this.support.bind(this, iter, event, runKey), 10);
      } else {
        this.fail(event, runKey);
      }
    }
  }

  startLegacy(event){
    this.initialForce = event.touches[0].force;
    this.supportLegacy(0, event, this.runKey, this.initialForce);
  }

  // this checks up to 6 times on a touch to see if the touch can read a force value
  // if the force value has changed it means the device supports pressure
  // more info from this issue https://github.com/yamartino/pressure/issues/15
  supportLegacy(iter, event, runKey, force){
    if(force !== this.initialForce){
      this._startPress(event);
      this.loopForce(event);
    } else if(iter <= 6) {
      iter++;
      setTimeout(this.supportLegacy.bind(this, iter, event, runKey, force), 10);
    } else{
      this.fail(event, runKey);
    }
  }

  loopForce(event){
    if(this.isPressed()) {
      this.touch = this.selectTouch(event);
      setTimeout(this.loopForce.bind(this, event), 10);
      this._changePress(this.touch.force, event);
    }
  }

  // link up the touch point to the correct element, this is to support multitouch
  selectTouch(event){
    if(event.touches.length === 1){
      return this.returnTouch(event.touches[0], event);
    } else {
      for(var i = 0; i < event.touches.length; i++){
        // if the target press is on this element
        if(event.touches[i].target === this.el || this.el.contains(event.touches[i].target)){
          return this.returnTouch(event.touches[i], event);
        }
      }
    }
  }

  // return the touch and run a start or end for deep press
  returnTouch(touch, event){
    this.deepPress(touch.force, event);
    return touch;
  }

}


/*
This adapter is for Macs with Force Touch trackpads.
*/

class AdapterForceTouch extends Adapter{

  constructor(el, block, options){
    super(el, block, options);
  }

  bindEvents(){
    this.add('webkitmouseforcewillbegin', this._startPress.bind(this));
    this.add('mousedown', this.support.bind(this));
    this.add('webkitmouseforcechanged', this.change.bind(this));
    this.add('webkitmouseforcedown', this._startDeepPress.bind(this));
    this.add('webkitmouseforceup', this._endDeepPress.bind(this));
    this.add('mouseleave', this._endPress.bind(this));
    this.add('mouseup', this._endPress.bind(this));
  }

  support(event){
    if(this.isPressed() === false){
      this.fail(event, this.runKey);
    }
  }

  change(event){
    if(this.isPressed() && event.webkitForce > 0){
      this._changePress(this.normalizeForce(event.webkitForce), event);
    }
  }

  // make the force the standard 0 to 1 scale and not the 1 to 3 scale
  normalizeForce(force){
    return this.reachOne(map(force, 1, 3, 0, 1));
  }

  // if the force value is above 0.995 set the force to 1
  reachOne(force){
    return force > 0.995 ? 1 : force;
  }

}


/*
This adapter is for devices that support pointer events.
*/

class AdapterPointer extends Adapter{

  constructor(el, block, options){
    super(el, block, options);
  }

  bindEvents(){
    this.add('pointerdown', this.support.bind(this));
    this.add('pointermove', this.change.bind(this));
    this.add('pointerup', this._endPress.bind(this));
    this.add('pointerleave', this._endPress.bind(this));
  }

  support(event){
    if(this.isPressed() === false){
      if(event.pressure === 0 || event.pressure === 0.5 || event.pressure > 1){
        this.fail(event, this.runKey);
      } else {
        this._startPress(event);
        this._changePress(event.pressure, event);
      }
    }
  }

  change(event){
    if(this.isPressed() && event.pressure > 0 && event.pressure !== 0.5){
      this._changePress(event.pressure, event);
      this.deepPress(event.pressure, event);
    }
  }

}


// This class holds the states of the the Pressure config
var Config = {

  // 'false' will make polyfill not run when pressure is not supported and the 'unsupported' method will be called
  polyfill: true,

  // milliseconds it takes to go from 0 to 1 for the polyfill
  polyfillSpeedUp: 1000,

  // milliseconds it takes to go from 1 to 0 for the polyfill
  polyfillSpeedDown: 0,

  // 'true' prevents the selecting of text and images via css properties
  preventSelect: true,

  // 'touch', 'mouse', or 'pointer' will make it run only on that type of device
  only: null,

  // this will get the correct config / option settings for the current pressure check
  get(option, options){
    return options.hasOwnProperty(option) ? options[option] : this[option];
  },

  // this will set the global configs
  set(options){
    for (var k in options) {
      if (options.hasOwnProperty(k) && this.hasOwnProperty(k) && k != 'get' && k != 'set') {
        this[k] = options[k];
      }
    }
  }

}


class Element{

  constructor(el, block, options){
    this.routeEvents(el, block, options);
    this.preventSelect(el, options);
  }

  routeEvents(el, block, options){
    var type = Config.get('only', options);
    // for devices that support pointer events
    if(supportsPointer && (type === 'pointer' || type === null)){
      this.adapter = new AdapterPointer(el, block, options).bindEvents();
    }
    // for devices that support 3D Touch
    else if(supportsTouch && (type === 'touch' || type === null)){
      this.adapter = new Adapter3DTouch(el, block, options).bindEvents();
    }
    // for devices that support Force Touch
    else if(supportsMouse && (type === 'mouse' || type === null)){
      this.adapter = new AdapterForceTouch(el, block, options).bindEvents();
    }
    // unsupported if it is requesting a type and your browser is of other type
    else{
      this.adapter = new Adapter(el, block).bindUnsupportedEvent();
    }
  }

  // prevent the default action of text selection, "peak & pop", and force touch special feature
  preventSelect(el, options){
    if(Config.get('preventSelect', options)){
      el.style.webkitTouchCallout = "none";
      el.style.webkitUserSelect = "none";
      el.style.khtmlUserSelect = "none";
      el.style.MozUserSelect = "none";
      el.style.msUserSelect = "none";
      el.style.userSelect = "none";
    }
  }

}


//------------------- Helpers -------------------//

// accepts jQuery object, node list, string selector, then called a setup for each element
var loopPressureElements = function(selector, closure, options = {}){
  // if a string is passed in as an element
  if(typeof selector === 'string' || selector instanceof String){
    var elements = document.querySelectorAll(selector);
    for (var i = 0; i < elements.length; i++) {
      new Element(elements[i], closure, options);
    }
  // if a single element object is passed in
  } else if(isElement(selector)){
    new Element(selector, closure, options);
  // if a node list is passed in ex. jQuery $() object
  } else {
    for (var i = 0; i < selector.length; i++) {
      new Element(selector[i], closure, options);
    }
  }
}

//Returns true if it is a DOM element
var isElement = function(o){
  return (
    typeof HTMLElement === "object" ? o instanceof HTMLElement : //DOM2
    o && typeof o === "object" && o !== null && o.nodeType === 1 && typeof o.nodeName==="string"
  );
}

// the map method allows for interpolating a value from one range of values to another
// example from the Arduino documentation: https://www.arduino.cc/en/Reference/Map
var map = function(x, in_min, in_max, out_min, out_max){
  return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}

var supportsMouse            = false;
var supportsTouch            = false;
var supportsPointer          = false;
var supportsTouchForce       = false;
var supportsTouchForceChange = false;

if (typeof window !== 'undefined') {
  // only attempt to assign these in a browser environment.
  // on the server, this is a no-op, like the rest of the library
  if (typeof Touch !== 'undefined') {
    // In Android, new Touch requires arguments.
    try {
      if (Touch.prototype.hasOwnProperty('force') || 'force' in new Touch()) {
        supportsTouchForce = true;
      }
    } catch (e) {}
  }
  supportsTouch            = 'ontouchstart'       in window.document && supportsTouchForce;
  supportsMouse            = 'onmousemove'        in window.document && !supportsTouch;
  supportsPointer          = 'onpointermove'      in window.document;
  supportsTouchForceChange = 'ontouchforcechange' in window.document;
}


//--------------------- Public API Section ---------------------//
// this is the Pressure Object, this is the only object that is accessible to the end user
// only the methods in this object can be called, making it the "public api"

var Pressure = {

  // targets any device with Force or 3D Touch
  set(selector, closure, options){
    loopPressureElements(selector, closure, options);
  },

  // set configuration options for global config
  config(options){
    Config.set(options);
  },

  // the map method allows for interpolating a value from one range of values to another
  // example from the Arduino documentation: https://www.arduino.cc/en/Reference/Map
  map(x, in_min, in_max, out_min, out_max){
    return map.apply(null, arguments);
  }

}

//--------------------- Public jQuery API Section ---------------------//

if($){

  $.fn.pressure = function(closure, options) {
    loopPressureElements(this, closure, options);
    return this;
  };

  $.pressureConfig = function(options){
    Config.set(options);
  };

  $.pressureMap = function(x, in_min, in_max, out_min, out_max) {
    return map.apply(null, arguments);
  };

} else {
  throw new Error( "Pressure jQuery requires jQuery to be loaded." );
}
