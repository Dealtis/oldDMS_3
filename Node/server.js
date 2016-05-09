var _ = require('lodash');
var express = require('express');
var app = express();


var server = app.listen(3300, function() {
    console.log('App listening on port 3300!');
});

var socket = require('socket.io');
var io = socket(server);

var users = [];
io.on('connection', function(socket) {
    console.log("new conn" + socket.id);
    //OnConn
    socket.emit('OnConn');
    socket.on('OnConnResponse', function(user) {
        socket.username = user;
        users.push(socket);
        console.log(_.map(users, 'id'));
        //updateUserList();
    });

    function updateUserList() {
      socket.broadcast.emit('userList',users);
    }
    //GPS
    socket.emit('askgps');
    socket.on('responsegps', function(gps) {
        console.log(gps);
    });

    //InsertPos
    socket.on('newPos', function(pos) {
        console.log(pos);
        socket.emit('sendPos');
    });

    //ClotureGrp
    socket.on('clotGrp', function(grp) {
        console.log(grp);
        socket.emit('sendclotGrp');
    });

    //InsertMsg
    socket.on('newMsg', function(msg) {
        console.log(msg);
        //socket.emit('sendMsg');
    });

    //ComPos
    socket.on('statPos', function(pos) {
        console.log(pos);
    });

    //NotifMsg
    socket.on('statMsg', function(msg) {
        console.log(msg);
    });

    //GetListUser
    socket.on('askUser', function() {
        console.log(msg);
        socket.emit('responseUser');
    });

    socket.on('disconnect', function() {
        console.log("Deconnexion de " + socket.id);
        _.pullAllBy(users, [{'id': socket.id}], 'id');
        console.log(_.map(users, 'id'));
    });
});


// socket.on('send', function(username,data) {
//   console.log(username);
//   var num = selectWhere(username);
//   console.log(num);
//   users[num].socket.emit("newEmit",data);
// })
