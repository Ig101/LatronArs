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
    vec4 backgroundTextureColor = texture2D(u_texture, v_backgroundTexCoord);

    /*
        mask
        r - shines
        g - silhouette
        b - colored
        a - active
        backgroundColor = light
    */

    float shinesAlpha = mask.r * maskTextureColor.r;

    if (mask.g != 0.0) {
        gl_FragColor = vec4(
            (1.0 - shinesAlpha) * ((maskTextureColor.g * color.r + (1.0 - maskTextureColor.g) * backgroundTextureColor.r) * backgroundColor.r) + shinesAlpha,
            (1.0 - shinesAlpha) * ((maskTextureColor.g * color.g + (1.0 - maskTextureColor.g) * backgroundTextureColor.g) * backgroundColor.g) + shinesAlpha,
            (1.0 - shinesAlpha) * ((maskTextureColor.g * color.b + (1.0 - maskTextureColor.g) * backgroundTextureColor.b) * backgroundColor.b),
            max(shinesAlpha, max(maskTextureColor.g, backgroundTextureColor.a))
        );
    } else if (mask.a != 0.0) {
        gl_FragColor = vec4(
            (1.0 - shinesAlpha) * ((textureColor.a * color.r + (1.0 - textureColor.a) * backgroundTextureColor.r) * backgroundColor.r) + shinesAlpha,
            (1.0 - shinesAlpha) * ((textureColor.a * color.g + (1.0 - textureColor.a) * backgroundTextureColor.g) * backgroundColor.g) + shinesAlpha,
            (1.0 - shinesAlpha) * ((textureColor.a * color.b + (1.0 - textureColor.a) * backgroundTextureColor.b) * backgroundColor.b),
            max(shinesAlpha, max(textureColor.a, backgroundTextureColor.a))
        );
    } else {
        gl_FragColor = vec4(
            (1.0 - shinesAlpha) * ((1.0 * textureColor.a * textureColor.r * ((1.0 - maskTextureColor.b) + 1.0 * maskTextureColor.b * color.r) + (1.0 - textureColor.a) * backgroundTextureColor.r) * backgroundColor.r) + shinesAlpha,
            (1.0 - shinesAlpha) * ((1.0 * textureColor.a * textureColor.g * ((1.0 - maskTextureColor.b) + 1.0 * maskTextureColor.b * color.g) + (1.0 - textureColor.a) * backgroundTextureColor.g) * backgroundColor.g) + shinesAlpha,
            (1.0 - shinesAlpha) * ((1.0 * textureColor.a * textureColor.b * ((1.0 - maskTextureColor.b) + 1.0 * maskTextureColor.b * color.b) + (1.0 - textureColor.a) * backgroundTextureColor.b) * backgroundColor.b),
            max(shinesAlpha, max(textureColor.a, backgroundTextureColor.a))
        );
    }
}