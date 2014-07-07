#region File Description
//-----------------------------------------------------------------------------
// Flying Kite
//
// Quickstarter for Wave University Tour 2014.
// Author: Wave Engine Team
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
#endregion

namespace FlyingOvniProject.Resources
{
    public class Textures
    {
        public enum GameAtlas
        {
            bg_cloud,
            bg_plane,
            bg_fish_kite_01,
            bg_fish_kite_02,
            kite_ball,
            obstacle_bottom,
            obstacle_top
        };

        public static readonly string KITE_ROPE = "Content/Kite/kite_rope_3x1.wpk";
        public static readonly string KITE_ANIMS = "Content/Kite/kite_anims.wpk";
        public static readonly string OVNI = "Content/Ovni/OVNI_01.wpk";
        public static readonly string OVNI_BOOST = "Content/Ovni/OVNI_boost.wpk";
        public static readonly string OVNI_WAVE = "Content/Ovni/OVNI_wave.wpk";
        public static readonly string KITE_ANIMS_XML = "Content/Kite/kite_anims.xml";
        public static readonly string GAME_ATLAS = "Content/Game.wpk";
        public static readonly string BG_IMAGE = "Content/bg_space_alpha.wpk";
        public static readonly string BG_MASK = "Content/bg_space_mask.wpk";

        public static readonly string KITE_COLLID = "Content/PixelCollider/fish_kite.wpk";
        public static readonly string OBSTACLE_TOP_COLLID = "Content/PixelCollider/obstacle_top.wpk";
        public static readonly string OBSTACLE_BOTTOM_COLLID = "Content/PixelCollider/obstacle_bottom.wpk";
               
        public static readonly string BEST_SCORE = "Content/Menus/best_score.wpk";
        public static readonly string BG_GAME_OVER = "Content/Menus/bg_gameover.wpk";
        public static readonly string GAME_OVER = "Content/Menus/gameover.wpk";
        public static readonly string LOGO = "Content/Menus/logo.wpk";
        public static readonly string PLAY_BUTTON = "Content/Menus/play_button.wpk";
        public static readonly string PLAY_BUTTON_PRESSED = "Content/Menus/play_button_pressed.wpk";
        public static readonly string STAR = "Content/Menus/star.wpk";
    }
}
