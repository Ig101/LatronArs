attribute vec2 a_position;
attribute vec2 a_texCoord;
attribute vec2 a_backgroundTexCoord;

uniform vec2 u_positionResolution;
uniform vec2 u_textureResolution;

varying vec2 v_texCoord;
varying vec2 v_backgroundTexCoord;
varying vec2 v_position;

void main() {
    vec2 resolutedPosition = a_position / u_positionResolution;
    vec2 transformedPosition = (resolutedPosition * 2.0) - 1.0;
    gl_Position = vec4(transformedPosition * vec2(1, -1), 0, 1);

    vec2 resolutedTexCoord = a_texCoord / u_textureResolution;
    vec2 resolutedBackgroundTexCoord = a_backgroundTexCoord / u_textureResolution;

    v_texCoord = resolutedTexCoord;
    v_backgroundTexCoord = resolutedBackgroundTexCoord;
    v_position = resolutedPosition;
}