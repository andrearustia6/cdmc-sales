var SF = (function (sf) {
    var objURL = function (url) {
        this.ourl = url || window.location.href;
        this.href = "";
        this.params = {};
        this.jing = "";
        this.init();
    }

    objURL.prototype.init = function () {
        var str = this.ourl;
        var index = str.indexOf("#");
        if (index > 0) {
            this.jing = str.substr(index);
            str = str.substring(0, index);
        }
        index = str.indexOf("?");
        if (index > 0) {
            this.href = str.substring(0, index);
            str = str.substr(index + 1);
            var parts = str.split("&");
            for (var i = 0; i < parts.length; i++) {
                var kv = parts[i].split("=");
                this.params[kv[0]] = kv[1];
            }
        }
        else {
            this.href = this.ourl;
            this.params = {};
        }
    }

    objURL.prototype.set = function (key, val) {
        this.params[key] = val;
    }

    objURL.prototype.remove = function (key) {
        this.params[key] = undefined;
    }

    objURL.prototype.url = function () {
        var strurl = this.href;
        var objps = [];
        for (var k in this.params) {
            if (this.params[k]) {
                objps.push(k + "=" + this.params[k]);
            }
        }
        if (objps.length > 0) {
            strurl += "?" + objps.join("&");
        }
        if (this.jing.length > 0) {
            strurl += this.jing;
        }
        return strurl;
    }

    objURL.prototype.get = function (key) {
        return this.params[key];
    }
    sf.URL = objURL;
    return sf;
} (SF || {}));

function stopEvent(evt) {
    var evt = evt || window.event;
    if (evt.preventDefault) {
        evt.preventDefault();
        evt.stopPropagation();
    } else {
        evt.returnValue = false;
        evt.cancelBubble = true;
    }
}
