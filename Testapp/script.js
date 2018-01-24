function test() {
    alert('blubb');
}

var magnitude = 0.0;

$(document).ready(function() {

  var scale = Math.min($(window).width(), $(window).height());

  $(document).mousemove(function(event) {
    $.ajax({
      type: "POST",
      url: "/data",
      data: "{\"copX\":" + ((event.pageX - $(window).width() / 2) / scale) + ", \"copZ\":" + ((event.pageY - $(window).height() / 2) / scale) + ", \"magnitude\": " + magnitude + "}"
    })
  });

  document.addEventListener('touchmove', function(event) {
    $.ajax({
      type: "POST",
      url: "/data",
      data: "{\"copX\":" + ((event.touches[0].pageX - $(window).width() / 2) / scale) + ", \"copZ\":" + ((event.touches[0].pageY - $(window).height() / 2) / scale) + ", \"magnitude\": " + magnitude + "}"
    })
  });

  $(document).pressure({
    change: function(force, event) {
      magnitude = force;
    },

    end: function(event) {
      magnitude = 0.0;
    }
  });
});
