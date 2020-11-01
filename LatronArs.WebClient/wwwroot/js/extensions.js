DOMGetBoundingClientRect = (element, parm) => { return element.getBoundingClientRect(); };

browserResize = {
    registerResizeCallback: function () {
        window.addEventListener("resize", browserResize.resized);
    },
    resized: function () {
        DotNet.invokeMethodAsync("LatronArs.WebClient", 'PushResize').then(data => data);
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
        console.log('TMA', id);
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

function downloadByteArray(content1, content2, content3) {
    var j = Blazor.platform.toUint8Array(content1);
    var j2 = Blazor.platform.toUint8Array(content2);
    var j3 = Blazor.platform.toUint8Array(content3);
}

function downloadFloatArray(content1, content2, content3) {
    var m = content1 + 12;
    var r = Module.HEAP32[m >> 2]
    var j = new Float32Array(Module.HEAPF32.buffer, m + 4, r);
  //  var j2 = Blazor.platform.toFloat32Array(content2);
  //  var j3 = Blazor.platform.toFloat32Array(content3);
}

function downloadFloatValues(content1, content2, content3) {
 //   console.log(content1);
  //  console.log(content2);
  //  var j2 = Blazor.platform.toFloat32Array(content2);
  //  var j3 = Blazor.platform.toFloat32Array(content3);
}