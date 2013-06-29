﻿var impact = function (div, json, settings) {
    settings = settings || {};
    var width = (+settings.width) || 100;
    var height = (+settings.height) || 35;
    var heightfunc = settings.heightfunc || Math.log;
    var datefont = settings.datefont || { "font": '9px "Arial"', stroke: "none", fill: "#fff" };
    var labelfont = settings.labelfont || { "font": '9px "Arial"', stroke: "none", fill: "#aaa" };
    var dateformatter = settings.dateformatter || function (dt) { return dt.getDate() + " " + ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"][dt.getMonth()] + " " + dt.getFullYear() };
    var mouseover = settings.mouseover || function () { };
    var x = 0,
        labels = {},
        pathes = {};

    function isIn(a, b) {
        var bucket = json.buckets[b];
        var len = bucket.i.length;
        for (var i = 0, len; i < len; i++) {
            if (bucket.i[i][0] == a) return true;
        }
        return false;
    }

    function findFirst(a, s, e, p) {
        for (var i = s; i != e; i += p) {
            if (isIn(a, i)) return i;
        }
    }

    function fillIn() {
        for (var i in json.authors) {
            var start = findFirst(i, 0, json.buckets.length - 1, 1);
            var end = findFirst(i, json.buckets.length - 1, 0, -1);

            for (var j = start, jj = end; j < jj; j++) {
                if (!isIn(i, j)) {
                    json.buckets[j].i.push([i, 0]);
                }
            }
        }
    }

    function measure() {
        var len = json.buckets.length;

        var maxItem = 1;
        for (var b = 0; b < len; b++) {
            var bucket = json.buckets[b];
            var items = bucket.i.length;

            for (var i = 0; i < items; i++) {
                maxItem = Math.max(maxItem, heightfunc(bucket.i[i][1]));
            }
        }

        var s = 35 / maxItem;

        var maxHeight = 0;
        for (var b = 0; b < len; b++) {
            var bucket = json.buckets[b];
            var items = bucket.i.length;

            var h = items - 1;
            for (var i = 0; i < items; i++) {
                h += Math.max(Math.round(heightfunc(bucket.i[i][1]) * s), 1);
            }
            h += 20;

            maxHeight = Math.max(maxHeight, h);
        }

        return { width: width * len, height: maxHeight, scale: s };
    }

    function build(scale) {
        var p, h;
        for (var j = 0, jj = json.buckets.length; j < jj; j++) {
            var users = json.buckets[j].i;
            h = 0;
            for (var i = 0, ii = users.length; i < ii; i++) {
                p = pathes[users[i][0]];
                if (!p) {
                    p = pathes[users[i][0]] = { f: [], b: [] };
                }
                p.f.push([x, h, users[i][1]]);
                p.b.unshift([x, h += Math.max(Math.round(heightfunc(users[i][1]) * scale), 1)]);
                h += 2;
            }
            var dt = new Date(json.buckets[j].d * 1000);
            var dtext = dateformatter(dt);
            r.text(x + (width * 0.25), h + 10, dtext).attr(labelfont);
            x += width;
        }
        var c = 0;
        for (var i in pathes) {
            labels[i] = r.set();
            var clr = Raphael.getColor();
            pathes[i].p = r.path().attr({ fill: clr, stroke: clr });
            var path = "M".concat(pathes[i].f[0][0], ",", pathes[i].f[0][1], "L", pathes[i].f[0][0] + (width * 0.5), ",", pathes[i].f[0][1]);
            var th = Math.round(pathes[i].f[0][1] + (pathes[i].b[pathes[i].b.length - 1][1] - pathes[i].f[0][1]) / 2 + 3);
            labels[i].push(r.text(pathes[i].f[0][0] + (width * 0.25), th, pathes[i].f[0][2]).attr(datefont));
            var X = pathes[i].f[0][0] + (width * 0.5),
                Y = pathes[i].f[0][1];
            for (var j = 1, jj = pathes[i].f.length; j < jj; j++) {
                path = path.concat("C", X + (width * 0.2), ",", Y, ",");
                X = pathes[i].f[j][0];
                Y = pathes[i].f[j][1];
                path = path.concat(X - (width * 0.2), ",", Y, ",", X, ",", Y, "L", X += (width * 0.5), ",", Y);
                th = Math.round(Y + (pathes[i].b[pathes[i].b.length - 1 - j][1] - Y) / 2 + 3);
                if (th - 9 > Y) {
                    labels[i].push(r.text(X - (width * 0.25), th, pathes[i].f[j][2]).attr(datefont));
                }
            }
            path = path.concat("L", pathes[i].b[0][0] + (width * 0.5), ",", pathes[i].b[0][1], ",", pathes[i].b[0][0], ",", pathes[i].b[0][1]);
            for (var j = 1, jj = pathes[i].b.length; j < jj; j++) {
                path = path.concat("C", pathes[i].b[j][0] + (width * 0.7), ",", pathes[i].b[j - 1][1], ",", pathes[i].b[j][0] + (width * 0.7), ",", pathes[i].b[j][1], ",", pathes[i].b[j][0] + (width * 0.5), ",", pathes[i].b[j][1], "L", pathes[i].b[j][0], ",", pathes[i].b[j][1]);
            }
            pathes[i].p.attr({ path: path + "z" });
            labels[i].hide();
            var current = null;
            (function (i) {
                pathes[i].p.mouseover(function () {
                    if (current != null) {
                        labels[current].hide();
                    }
                    current = i;
                    labels[i].show();
                    pathes[i].p.toFront();
                    labels[i].toFront();
                    mouseover(json.authors[i]);
                });
            })(i);
        }
    }

    fillIn();
    var m = measure();
    var r = Raphael(div, m.width, m.height);
    build(m.scale);
};