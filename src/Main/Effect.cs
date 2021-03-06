using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ReiseZumGrundDesSees
{
    interface IEffect
    {
        Matrix WorldMatrix { set; get; }
        Matrix ViewMatrix { set; get; }
        Matrix ProjectionMatrix { set; get; }

        Texture2D Texture { set; get; }

        Color Color { get; set; }

        VertexFormat VertexFormat { get; set; }

        Effect Effect { get; }
    }

    class DefaultEffect : IEffect
    {
        public Matrix WorldMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }

        public Texture2D Texture { get; set; }

        public Color Color { get; set; }

        public VertexFormat VertexFormat { get; set; }

        public Effect Effect
        {
            get
            {
                effect.Parameters["Matrix"].SetValue(WorldMatrix * ViewMatrix * ProjectionMatrix);
                if (effect.Parameters.Where(_param => _param.Name.Equals("textureSampler+otherTexture")).Any())
                    effect.Parameters["textureSampler+otherTexture"].SetValue(Texture);
                return effect;
            }
        }
        private Effect effect;

        public DefaultEffect(Effect _effect)
        {
            effect = _effect;
        }
    }

    class ShadowEffect : IEffect
    {
        private float animationValue = 0f;

        public Matrix WorldMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        public Matrix NearLightMatrix { get; set; }
        public Matrix FarLightMatrix { get; set; }

        public Texture2D Texture { get; set; }
        public Texture2D NearLightTexture { get; set; }
        public Texture2D FarLightTexture { get; set; }

        public Color Color { get; set; }

        public Vector2 AnimationValue { get; set; }

        public VertexFormat VertexFormat { get; set; }

        private Effect basicEffect;
        private Effect colorEffect;
        private Effect textureEffect;
        private Effect worldEffect;

        public void Update(GameTime _time)
        {
            animationValue += (float)_time.ElapsedGameTime.TotalMilliseconds * 0.005f;
        }

        private void Apply(Effect _effect)
        {
            if (_effect == worldEffect)
            {
                _effect.Parameters["AnimationValue"].SetValue(animationValue);
            }

            _effect.Parameters["Matrix"].SetValue(WorldMatrix * ViewMatrix * ProjectionMatrix);
            if (_effect == textureEffect || _effect == worldEffect)
#if LINUX
                    _effect.Parameters["textureSampler+otherTexture"].SetValue(Texture);
#elif WINDOWS
                _effect.Parameters["otherTexture"].SetValue(Texture);
#endif
            if (_effect == colorEffect)
                _effect.Parameters["Color"].SetValue(Color.ToVector4());
            _effect.Parameters["NearLightMatrix"].SetValue(WorldMatrix * NearLightMatrix);
            _effect.Parameters["FarLightMatrix"].SetValue(WorldMatrix * FarLightMatrix);

#if LINUX
                _effect.Parameters["nearShadowSampler+nearShadowTexture"].SetValue(NearLightTexture);
                _effect.Parameters["farShadowSampler+farShadowTexture"].SetValue(FarLightTexture);
#elif WINDOWS
            _effect.Parameters["nearShadowTexture"].SetValue(NearLightTexture);
            _effect.Parameters["farShadowTexture"].SetValue(FarLightTexture);
#endif
        }

        public Effect Effect
        {
            get
            {
                if (VertexFormat == VertexFormat.Position)
                {
                    Apply(basicEffect);
                    return basicEffect;
                }
                else if (VertexFormat == VertexFormat.PositionColor)
                {
                    Apply(colorEffect);
                    return colorEffect;
                }
                else if (VertexFormat == VertexFormat.PositionTexture)
                {
                    Apply(textureEffect);
                    return textureEffect;
                }
                else if (VertexFormat == VertexFormat.World)
                {
                    Apply(worldEffect);
                    return worldEffect;
                }
                else throw new NotSupportedException();
            }
        }

        public ShadowEffect(ContentManager _contentManager)
        {
            basicEffect = _contentManager.Load<Effect>(ContentRessources.EFFECT_SHADOW_BASIC);
            colorEffect = _contentManager.Load<Effect>(ContentRessources.EFFECT_SHADOW_COLOR);
            textureEffect = _contentManager.Load<Effect>(ContentRessources.EFFECT_SHADOW_TEXTURE);
            worldEffect = _contentManager.Load<Effect>(ContentRessources.EFFECT_SHADOW_WORLD);
        }
    }

    enum VertexFormat
    {
        Position,
        PositionTexture,
        PositionColor,
        World
    }
}
