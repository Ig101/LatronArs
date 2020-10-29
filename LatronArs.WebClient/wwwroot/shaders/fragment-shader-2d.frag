precision mediump float;

uniform sampler2D u_color;
uniform sampler2D u_backgroundColor;
uniform sampler2D u_maskMap;

uniform sampler2D u_texture;
uniform sampler2D u_mask;

varying vec2 v_texCoord;
varying vec2 v_backgroundTexCoord;
varying vec2 v_position;

void main() {
    vec4 color = texture2D(u_color, v_position);
    vec3 backgroundColor = texture2D(u_backgroundColor, v_position).rgb;
    vec4 mask = texture2D(u_maskMap, v_position);

    vec4 textureColor = texture2D(u_texture, v_texCoord);
    vec4 maskTextureColor = texture2D(u_mask, v_texCoord);
    vec3 backgroundTextureColor = texture2D(u_texture, v_backgroundTexCoord);

    /*
        mask
        r - shines
        g - silhouette
        b - colored
        a - active
        backgroundColor = light
    */

    float shinesAlpha = mask.r != 0.0;

    if (mask.g != 0.0) {
        gl_FragColor = vec(
            (maskTextureColor.g * color.r + (1.0 - maskTextureColor.g) * backgroundTextureColor.r) * backgroundColor.r,
            (maskTextureColor.g * color.g + (1.0 - maskTextureColor.g) * backgroundTextureColor.g) * backgroundColor.g,
            (maskTextureColor.g * color.b + (1.0 - maskTextureColor.g) * backgroundTextureColor.b) * backgroundColor.b,
            max(textureColor.a, backgroundTextureColor.a)
        );
    } else if (mask.a != 0.0) {
        gl_FragColor = vec(
            (maskTextureColor.a * color.r + (1.0 - maskTextureColor.a) * backgroundTextureColor.r) * backgroundColor.r,
            (maskTextureColor.a * color.g + (1.0 - maskTextureColor.a) * backgroundTextureColor.g) * backgroundColor.g,
            (maskTextureColor.a * color.b + (1.0 - maskTextureColor.a) * backgroundTextureColor.b) * backgroundColor.b,
            max(textureColor.a, backgroundTextureColor.a)
        );
    } else {
        gl_FragColor = vec(
            (textureColor.a * ((1.0 - maskTextureColor.b) * textureColor.r + maskTextureColor.b * color.r) + (1.0 - textureColor.a) * backgroundTextureColor.r) * backgroundColor.r,
            (textureColor.a * ((1.0 - maskTextureColor.b) * textureColor.g + maskTextureColor.b * color.g) + (1.0 - textureColor.a) * backgroundTextureColor.g) * backgroundColor.g,
            (textureColor.a * ((1.0 - maskTextureColor.b) * textureColor.b + maskTextureColor.b * color.b) + (1.0 - textureColor.a) * backgroundTextureColor.b) * backgroundColor.b,
            max(textureColor.a, backgroundTextureColor.a)
        );
    }
}