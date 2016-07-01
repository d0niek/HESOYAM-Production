using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using HESOYAM_Production;

namespace App.Animation
{

    /// <summary>
    /// An encloser for an XNA model that we will use that includes support for
    /// bones, animation, and some manipulations.
    /// </summary>
    public class AnimatedObject: GameObject
    {
        #region Fields

        /// <summary>
        /// Extra data associated with the XNA model
        /// </summary>
        protected ModelExtra modelExtra = null;

        /// <summary>
        /// The model bones
        /// </summary>
        protected List<Bone> bones = new List<Bone>();

        /// <summary>
        /// An associated animation clip player
        /// </summary>
        protected AnimationPlayer player = null;

        #endregion

        #region Properties

        /// <summary>
        /// The underlying bones for the model
        /// </summary>
        public List<Bone> Bones { get { return bones; } }

        public Dictionary<String,AnimationClip> Clips { get;}

        #endregion

        #region Construction and Loading

        /// <summary>
        /// Constructor. Creates the model from an XNA model
        /// </summary>
        /// <param name="assetName">The name of the asset for this model</param>
        public AnimatedObject(
            Engine game,
            string name,
            Model model,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game, name, model, position, rotation, scale)
        {
            this.Clips = new Dictionary<string, AnimationClip>();
            modelExtra = model.Tag as ModelExtra;
            System.Diagnostics.Debug.Assert(modelExtra != null);
            ObtainBones();
        }

        public AnimatedObject(
            Engine game,
            string name,
            Vector3 position = default(Vector3), 
            Vector3 rotation = default(Vector3), 
            Vector3? scale = null
        ) : base(game, name, position, rotation, scale)
        {
            this.Clips = new Dictionary<string, AnimationClip>();
        }


        #endregion

        #region Bones Management

        /// <summary>
        /// Get the bones from the model and create a bone class object for
        /// each bone. We use our bone class to do the real animated bone work.
        /// </summary>
        protected void ObtainBones()
        {
            bones.Clear();
            foreach (ModelBone bone in model.Bones) {
                // Create the bone object and add to the heirarchy
                Bone newBone = new Bone(
                                   bone.Name,
                                   bone.Transform,
                                   bone.Parent != null ? bones[bone.Parent.Index] : null);

                // Add to the bones for this model
                bones.Add(newBone);
            }
        }

        /// <summary>
        /// Find a bone in this model by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Bone FindBone(string name)
        {
            foreach (Bone bone in Bones) {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }

        #endregion

        #region Animation Management

        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip)
        {
            if (this.player != null && this.player.Clip == clip) {
                return this.player;
            }

            player = new AnimationPlayer(clip, this);
            return player;
        }

        public AnimationPlayer PlayClip(String name)
        {
            return PlayClip(this.Clips[name]);
        }

        #endregion

        #region Updating

        /// <summary>
        /// Update animation for the model.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (player != null && this.game.PlayMode) {
                player.Update(gameTime);
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Draw the model
        /// </summary>
        /// <param name="graphics">The graphics device to draw on</param>
        /// <param name="camera">A camera to determine the view</param>
        /// <param name="world">A world matrix to place the model</param>
        public override void Draw(GameTime gameTime)
        {
            
            if (model == null)
                return;

            Matrix[] boneTransforms = new Matrix[bones.Count];

            for (int i = 0; i < bones.Count; i++) {
                Bone bone = bones[i];
                bone.ComputeAbsoluteTransform();

                boneTransforms[i] = bone.AbsoluteTransform;
            }

            //
            // Determine the skin transforms from the skeleton
            //

            Matrix[] skeleton = new Matrix[modelExtra.Skeleton.Count];

            for (int s = 0; s < modelExtra.Skeleton.Count; s++) {
                Bone bone = bones[modelExtra.Skeleton[s]];
                skeleton[s] = bone.SkinTransform * bone.AbsoluteTransform;
            }

            // Draw the model.
            foreach (ModelMesh modelMesh in model.Meshes) {
                foreach (Effect effect in modelMesh.Effects) {
                    if (effect is BasicEffect) {
                        BasicEffect beffect = effect as BasicEffect;
                        beffect.World = boneTransforms[modelMesh.ParentBone.Index]
                    * Matrix.CreateRotationY(this.rotation.Y)
                    * Matrix.CreateRotationX(this.rotation.X)
                    * Matrix.CreateRotationZ(this.rotation.Z)
                    * Matrix.CreateScale(this.scale)
                    * Matrix.CreateTranslation(this.position);;
                        beffect.View = this.game.Camera.ViewMatrix;
                        beffect.Projection = this.game.Camera.ProjectionMatrix;
                        beffect.EnableDefaultLighting();
                        beffect.PreferPerPixelLighting = true;

                        if(active)
                        {
                            beffect.AmbientLightColor = new Vector3(0, 0, 255);
                        }
                        else if(Hover)
                        {
                            beffect.AmbientLightColor = new Vector3(0, 255, 0);
                        }
                        else
                        {
                            beffect.AmbientLightColor = Vector3.Zero;
                        }
                    }

                    if (effect is SkinnedEffect) {
                        SkinnedEffect seffect = effect as SkinnedEffect;
                        seffect.World = boneTransforms[modelMesh.ParentBone.Index]
                    * Matrix.CreateRotationY(this.rotation.Y)
                    * Matrix.CreateRotationX(this.rotation.X)
                    * Matrix.CreateRotationZ(this.rotation.Z)
                    * Matrix.CreateScale(this.scale)
                    * Matrix.CreateTranslation(this.position);
                        seffect.View = this.game.Camera.ViewMatrix;
                        seffect.Projection = this.game.Camera.ProjectionMatrix;
                        //seffect.EnableDefaultLighting();
                        seffect.PreferPerPixelLighting = true;
                        seffect.SetBoneTransforms(skeleton);

                        if(active)
                        {
                            seffect.AmbientLightColor = new Vector3(0, 0, 255);
                        }
                        else if(Hover)
                        {
                            seffect.AmbientLightColor = new Vector3(0, 255, 0);
                        }
                        else
                        {
                            seffect.AmbientLightColor = Vector3.Zero;
                        }
                    }

                    this.DrawTexture(effect);
                }

                modelMesh.Draw();
            }
        }

        protected void DrawTexture(Effect effect)
        {
            if (this.texture != null) {
                if (effect is BasicEffect) {
                    BasicEffect beffect = effect as BasicEffect;
                    beffect.TextureEnabled = true;
                    beffect.Texture = this.texture;
                }
                if (effect is SkinnedEffect) {
                    SkinnedEffect seffect = effect as SkinnedEffect;
                    seffect.Texture = this.texture;
                    seffect.EmissiveColor = new Vector3(0.7f,0.7f,0.7f);
                }
            }
        }


        #endregion

    }
}



