﻿#region License
/*
    PloobsEngine Game Engine Version 0.3 Beta
    Copyright (C) 2011  Ploobs

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PloobsEngine.Engine;
using PloobsEngine.Utils;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Collision.Shapes;
using PloobsEngine.Physic2D.Farseer;

namespace PloobsEngine.Modelo2D
{
    public class SpriteFarseer : IModelo2D
    {
        static FarseerAsset2DCreator assetCreator = null;

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="vertices"></param>
        /// <param name="color"></param>
        /// <param name="isInSimulationUnits"></param>
        /// <param name="materialScale"></param>
        public SpriteFarseer(GraphicFactory factory, Vertices vertices, Color color, bool isInSimulationUnits = false,float materialScale = 1)
            : base(ModelType.Texture)
        {
            if(assetCreator ==null)
                assetCreator  = new FarseerAsset2DCreator(factory);

            if (isInSimulationUnits)
            {
                for (int i = 0; i < vertices.Count; i++)
                {
                    vertices[i] = ConvertUnits.ToSimUnits(vertices[i]);
                }
            }

            Texture = assetCreator.CreateTextureFromVertices(vertices,color, materialScale);
            Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="shape"></param>
        /// <param name="color"></param>
        /// <param name="isInSimulationUnits"></param>
        /// <param name="materialScale"></param>
        public SpriteFarseer(GraphicFactory factory, PolygonShape shape, Color color, bool isInSimulationUnits = false, float materialScale = 1)
            : base(ModelType.Texture)
        {
            if (assetCreator == null)
                assetCreator = new FarseerAsset2DCreator(factory);

            if (isInSimulationUnits)
            {
                for (int i = 0; i < shape.Vertices.Count; i++)
                {
                    shape.Vertices[i] = ConvertUnits.ToSimUnits(shape.Vertices[i]);
                }
            }

            Texture = assetCreator.CreateTextureFromVertices(shape.Vertices, color, materialScale);
            Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="factory"></param>
       /// <param name="radius"></param>
       /// <param name="color"></param>
       /// <param name="isInSimulationUnits"></param>
       /// <param name="materialScale"></param>
         public SpriteFarseer(GraphicFactory factory, float radius, Color color, bool isInSimulationUnits = false, float materialScale = 1)
            : base(ModelType.Texture)
        {
            if (assetCreator == null)
                assetCreator = new FarseerAsset2DCreator(factory);

            if (isInSimulationUnits)
            {
                radius = ConvertUnits.ToSimUnits(radius);
            }


            Texture = assetCreator.CreateCircleTexture(radius, color, materialScale);
            Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        }

        public SpriteFarseer(Texture2D texture)
            : base(ModelType.Texture)
        {
            Texture = texture;
            Origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        }
    }
}
