const http = require('http');
const fs = require('fs');
const net = require('net');

const hostname = '0.0.0.0';
const port = 3000;

var copX = 0.0;
var copZ = 0.0;
var magnitude = 0.0;

const server = http.createServer(function(req, res) {

  if (req.method === 'GET' && req.url === '/') {
    fs.readFile('index.html',function (err, data){
      res.writeHead(200, {'Content-Type': 'text/html','Content-Length':data.length});
      res.write(data);
      res.end();
    });
  } else if (req.method === 'GET' && req.url === '/script.js') {
    fs.readFile('script.js',function (err, data){
      res.writeHead(200, {'Content-Type': 'text/javascript','Content-Length':data.length});
      res.write(data);
      res.end();
    });
  } else if (req.method === 'GET' && req.url === '/jquery.js') {
    fs.readFile('jquery.js',function (err, data){
      res.writeHead(200, {'Content-Type': 'text/javascript','Content-Length':data.length});
      res.write(data);
      res.end();
    });
  } else if (req.method === 'GET' && req.url === '/pressure.js') {
    fs.readFile('pressure.js',function (err, data){
      res.writeHead(200, {'Content-Type': 'text/javascript','Content-Length':data.length});
      res.write(data);
      res.end();
    });
  } else if (req.method === 'POST' && req.url === '/data') {
    var body = '';
    req.on('data', function (data) {
      body += data;
    });
    req.on('end', function () {
      data = JSON.parse(body);
      copX = data.copX;
      copZ = data.copZ;
      magnitude = data.magnitude;
      console.log(copX + " " + copZ + " " + magnitude);
      res.statusCode = 200;
      res.end();
    });
  } else {
    res.statusCode = 404;
    res.end();
  }
});

server.listen(port, hostname, function() {

  console.log("\nIP addresses this server can be accessed by:\n")

  var interfaces = require('os').networkInterfaces();

  for (var device in interfaces) {

    var addresses = [];

    for (var address of interfaces[device]) {
      if(address.family == "IPv4") {
        addresses.push(address.address);
      }
    }

    if(addresses.length > 0) {
      console.log("-> " + device);
      for(var address of addresses) {
        console.log("  " + address);
      }
    }
  }

  console.log("\n");
});

function writePacket(packet, buffer) {
  buffer.writeUInt32LE(packet.packetType, 0);
  buffer.writeUInt32LE(packet.nrInputs, 4);
  buffer.writeUInt32LE(packet.nrOutputs, 8);
  buffer.writeUInt32LE(packet.clientIndex, 12);
  buffer.write(packet.clientName, 16, 256, "utf8");
  for(var i = 0; i < 256; i++) {
    buffer.writeFloatBE(packet.values[i], 272 + i*4);
  }
}

//Dummy server to just report a client ID of 1
var dummyServer = net.createServer(function(socket) {

  var buffer = Buffer(1296);

  var packet = {
    packetType: 1,
    nrInputs: 4,
    nrOutputs: 6,
    clientIndex: 1,
    clientName: "",
    values: new Float32Array(256).fill(0.0)
  }

  writePacket(packet, buffer);

  socket.write(buffer);
  socket.pipe(socket);


}).listen(3910);

var dataSocketConnected = false;

net.createServer(function(socket) {

  dataSocketConnected = true;

  sendData(socket);

  socket.on('data', function(data) {

  });

  socket.on('end', function(socket) {
    dataSocketConnected = false;
  });
}).listen(3911);

function sendData(socket) {

  var buffer = Buffer(1296);

  var packet = {
    packetType: 2,
    nrInputs: 0,
    nrOutputs: 0,
    clientIndex: 1,
    clientName: "",
    values: new Array(256).fill(0.0)
  }

  packet.values[0] = copX;
  packet.values[1] = copZ;
  packet.values[2] = magnitude;

  writePacket(packet, buffer);
  //socket.pipe(socket);

  if(dataSocketConnected) {
    socket.write(buffer);
    setTimeout(sendData, 10, socket);
  }
}
