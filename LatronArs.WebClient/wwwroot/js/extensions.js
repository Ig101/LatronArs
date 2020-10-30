DOMGetBoundingClientRect = (element, parm) => { return element.getBoundingClientRect(); };

browserResize = {
    registerResizeCallback: function () {
        window.addEventListener("resize", browserResize.resized);
    },
    resized: function () {
        DotNet.invokeMethodAsync("LatronArs.WebClient", 'PushResize').then(data => data);
    }
}

var textureCanvasContext;
texture = {

    createCanvas: function (width, height) {
        textureCanvas = document.createElement('canvas'); 
        document.body.appendChild(textureCanvas);
        textureCanvas.width = width;
        textureCanvas.height = height;
        textureCanvasContext = charsSubCanvas.getContext('2d');
        textureCanvasContext.clearRect(0, 0, width, height);
    },
    drawImage: function (src, x, y) {
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
    getTexture: function(sx, sy, sw, sh) {
        return textureCanvasContext.getImageData(sx, sy, sw, sh);
    }
}