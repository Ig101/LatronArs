DOMGetBoundingClientRect = (element, parm) => { return element.getBoundingClientRect(); };

eventsExtensions = {
    registerEventsCallback: function () {
        window.addEventListener('resize', eventsExtensions.resized);
        window.addEventListener('keydown', eventsExtensions.keyDowned);
        window.addEventListener('keyup', eventsExtensions.keyUpped);
        window.addEventListener('contextmenu', eventsExtensions.cancelAction);
        window.addEventListener('onbeforeunload', eventsExtensions.cancelAction);
    },
    resized: function () {
        DotNet.invokeMethodAsync('LatronArs.WebClient', 'PushResize').then(data => data);
    },
    keyDowned: function (e) {
        if (e.code[0] !== 'F')
        {
            e.preventDefault();
            DotNet.invokeMethod('LatronArs.WebClient', 'PushKeyDown', {
                code: e.code,
                altKey: e.altKey,
                ctrlKey: e.ctrlKey,
                shiftKey: e.shiftKey
            });
        }
    },
    keyUpped: function (e) {
        if (e.code[0] !== 'F')
        {
            e.preventDefault();
            DotNet.invokeMethod('LatronArs.WebClient', 'PushKeyUp', {
                code: e.code
            });
        }
    },
    cancelAction: function (e) {
        e.preventDefault();
    }
}

var textureCanvas;
var textureCanvasContext;
var maskCanvas
var maskCanvasContext;
var sceneTextures = [];
var sceneMasks = [];
textureExtensions = {

    createCanvas: function (width, height) {
        textureCanvas = document.createElement('canvas'); 
        document.body.appendChild(textureCanvas);
        textureCanvas.width = width;
        textureCanvas.height = height;
        textureCanvasContext = textureCanvas.getContext('2d');
        textureCanvasContext.clearRect(0, 0, width, height);

        maskCanvas = document.createElement('canvas'); 
        document.body.appendChild(maskCanvas);
        maskCanvas.width = width;
        maskCanvas.height = height;
        maskCanvasContext = maskCanvas.getContext('2d');
        maskCanvasContext.clearRect(0, 0, width, height);
    },
    drawTexture: function (src, x, y) {
        return new Promise((resolve, reject) => {
            let img = new Image();
            img.onload = () => {
                textureCanvasContext.drawImage(img, x, y);
                resolve(img.height)
            };
            img.onerror = reject;
            img.src = src;
        });
    },
    drawMask: function (src, x, y) {
        return new Promise((resolve, reject) => {
            let img = new Image();
            img.onload = () => {
                maskCanvasContext.drawImage(img, x, y);
                resolve(img.height)
            };
            img.onerror = reject;
            img.src = src;
        });
    },
    buildTextureAndMask: function(canvas, id) {
        var gl = canvas.getContext('webgl');
        var texture = gl.createTexture();
        gl.bindTexture(gl.TEXTURE_2D, texture);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
        gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, textureCanvas);
        sceneTextures[id] = texture;

        var mask = gl.createTexture();
        gl.bindTexture(gl.TEXTURE_2D, mask);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
        gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, maskCanvas);
        sceneMasks[id] = mask;
    },
    getTexture: function(id) {
        return sceneTextures[id];
    },
    getMask: function(id) {
        return sceneMasks[id];
    }
}