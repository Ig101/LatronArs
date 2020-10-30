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
texture = {

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
    buildTexture: function(canvas) {
        var gl = canvas.getContext('webgl');
        console.log(gl);
        gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, textureCanvas);
        return gl;
    },
    buildMask: function(canvas) {
        var gl = canvas.getContext('webgl');
        gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, maskCanvas);
        return gl;
    }
}