
var contexts = [];
var scenePrograms = [];

function toFloat32Array(array) {
    var m = array + 12;
    var r = Module.HEAP32[m >> 2]
    return new Float32Array(Module.HEAPF32.buffer, m + 4, r);
}


var currentCanvasId;
var currentContext;
var currentVertices;
var currentForegrounds;
var currentBackgrounds;
var currentColors;
var currentBackgroundColors;
var currentMasks;
var currentCameraX;
var currentCameraY;
var currentColorsWidth;
var currentColorsHeight;
var currentTextureWidth;
var currentTextureHeight;

sceneExtensions = {
    registerWebGLContext: function(canvas, id, vertexCode, fragmentCode) {
        var gl = canvas.getContext('webgl');
        const vertexShader = gl.createShader(gl.VERTEX_SHADER);
        gl.shaderSource(vertexShader, vertexCode);
        gl.compileShader(vertexShader);
        if (!gl.getShaderParameter(vertexShader, gl.COMPILE_STATUS)) {
          console.error(gl.getShaderInfoLog(vertexShader));
          gl.deleteShader(vertexShader);
          return;
        }

        const fragmentShader = gl.createShader(gl.FRAGMENT_SHADER);
        gl.shaderSource(fragmentShader, fragmentCode);
        gl.compileShader(fragmentShader);
        if (!gl.getShaderParameter(fragmentShader, gl.COMPILE_STATUS)) {
          console.error(gl.getShaderInfoLog(fragmentShader));
          gl.deleteShader(fragmentShader);
          return;
        }

        const program = gl.createProgram();
        gl.attachShader(program, vertexShader);
        gl.attachShader(program, fragmentShader);
        gl.linkProgram(program);
        if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
          console.error(gl.getProgramInfoLog(program));
          gl.deleteProgram(program);
          return;
        }
        scenePrograms[id] = program;
        contexts[id] = gl;
    },
    setupContext: function(id) {
        currentContext = contexts[id];
        currentCanvasId = id;
    },
    setupVertices: function(vertices, foregrounds, backgrounds) {
        currentVertices = toFloat32Array(vertices);
        currentForegrounds = toFloat32Array(foregrounds);
        currentBackgrounds = toFloat32Array(backgrounds);
    },
    setupColors: function(colors, backgrounds, masks) {
        currentColors = Blazor.platform.toUint8Array(colors);
        currentBackgroundColors = Blazor.platform.toUint8Array(backgrounds);
        currentMasks = Blazor.platform.toUint8Array(masks);
    },
    setupXAxle: function(camera, colors, texture) {
        currentCameraX = camera;
        currentColorsWidth = colors;
        currentTextureWidth = texture;
    },
    setupYAxle: function(camera, colors, texture) {
        currentCameraY = camera;
        currentColorsHeight = colors;
        currentTextureHeight = texture;
    },
    draw: function(width, height) {
        var gl = currentContext;
        var program = scenePrograms[currentCanvasId];
        var texture = textureExtensions.getTexture(currentCanvasId);
        var mask = textureExtensions.getMask(currentCanvasId);

        gl.clearColor(0, 0, 0, 1);

        var positionLocation = gl.getAttribLocation(program, 'a_position');
        var texcoordLocation = gl.getAttribLocation(program, 'a_texCoord');
        var backgroundTexcoordLocation = gl.getAttribLocation(program, 'a_backgroundTexCoord');
      
        var positionBuffer = gl.createBuffer();
        gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
        gl.bufferData(gl.ARRAY_BUFFER, currentVertices, gl.STATIC_DRAW);
      
        var texcoordBuffer = gl.createBuffer();
        gl.bindBuffer(gl.ARRAY_BUFFER, texcoordBuffer);
        gl.bufferData(gl.ARRAY_BUFFER, currentForegrounds, gl.STATIC_DRAW);
      
        var backgroundTexcoordBuffer = gl.createBuffer();
        gl.bindBuffer(gl.ARRAY_BUFFER, backgroundTexcoordBuffer);
        gl.bufferData(gl.ARRAY_BUFFER, currentBackgrounds, gl.STATIC_DRAW);
      
        var colorTexture = gl.createTexture();
        gl.bindTexture(gl.TEXTURE_2D, colorTexture);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
        gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, currentColorsWidth, currentColorsHeight, 0, gl.RGBA, gl.UNSIGNED_BYTE, currentColors);
      
        var backgroundTexture = gl.createTexture();
        gl.bindTexture(gl.TEXTURE_2D, backgroundTexture);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
        gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, currentColorsWidth, currentColorsHeight, 0, gl.RGBA, gl.UNSIGNED_BYTE, currentBackgroundColors);
      
        var maskTexture = gl.createTexture();
        gl.bindTexture(gl.TEXTURE_2D, maskTexture);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
        gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, currentColorsWidth, currentColorsHeight, 0, gl.RGBA, gl.UNSIGNED_BYTE, currentMasks);
      
        var positionResolutionLocation = gl.getUniformLocation(program, 'u_positionResolution');
        var textureResolutionLocation = gl.getUniformLocation(program, 'u_textureResolution');
      
        gl.viewport(currentCameraX, currentCameraY, width, height);
        gl.useProgram(program);
      
        gl.enableVertexAttribArray(positionLocation);
        gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
        gl.vertexAttribPointer(positionLocation, 2, gl.FLOAT, false, 0, 0);
      
        gl.enableVertexAttribArray(texcoordLocation);
        gl.bindBuffer(gl.ARRAY_BUFFER, texcoordBuffer);
        gl.vertexAttribPointer(texcoordLocation, 2, gl.FLOAT, false, 0, 0);
      
        gl.enableVertexAttribArray(backgroundTexcoordLocation);
        gl.bindBuffer(gl.ARRAY_BUFFER, backgroundTexcoordBuffer);
        gl.vertexAttribPointer(backgroundTexcoordLocation, 2, gl.FLOAT, false, 0, 0);
      
        gl.uniform2f(positionResolutionLocation, width, height);
        gl.uniform2f(textureResolutionLocation, currentTextureWidth, currentTextureHeight);
      
        var colorLocation = gl.getUniformLocation(program, 'u_color');
        var backgroundLocation = gl.getUniformLocation(program, 'u_backgroundColor');
        var maskMapLocation = gl.getUniformLocation(program, 'u_maskMap');
      
        var textureLocation = gl.getUniformLocation(program, 'u_texture');
        var maskLocation = gl.getUniformLocation(program, 'u_mask');
      
        gl.uniform1i(colorLocation, 0);
        gl.uniform1i(backgroundLocation, 1);
        gl.uniform1i(maskMapLocation, 2);
        gl.uniform1i(textureLocation, 3);
        gl.uniform1i(maskLocation, 4);
      
        gl.activeTexture(gl.TEXTURE0);
        gl.bindTexture(gl.TEXTURE_2D, colorTexture);
        gl.activeTexture(gl.TEXTURE1);
        gl.bindTexture(gl.TEXTURE_2D, backgroundTexture);
        gl.activeTexture(gl.TEXTURE2);
        gl.bindTexture(gl.TEXTURE_2D, maskTexture);
        gl.activeTexture(gl.TEXTURE3);
        gl.bindTexture(gl.TEXTURE_2D, texture);
        gl.activeTexture(gl.TEXTURE4);
        gl.bindTexture(gl.TEXTURE_2D, mask);
      
        gl.enable(gl.BLEND);
        gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);
      
        gl.drawArrays(gl.TRIANGLES, 0, currentVertices.length / 2);
    }
}