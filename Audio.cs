using System.Media;

namespace Minesweeper;

public static class Audio
{
    public static readonly SoundPlayer sStart = new(Const.Sounds.start);
    public static readonly SoundPlayer sWin = new(Const.Sounds.win);
    public static readonly SoundPlayer sExplosion = new(Const.Sounds.explosion);
    public static readonly SoundPlayer sIntermission = new(Const.Sounds.intermission);
    public static readonly SoundPlayer sBlip1 = new(Const.Sounds.blip1);
    public static readonly SoundPlayer sBlip2 = new(Const.Sounds.blip2);
    public static readonly SoundPlayer sBlip3 = new(Const.Sounds.blip3);
}
