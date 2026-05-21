using Godot;
using System;

//Constants holds all the string values in the game to prevent error
public static class Constants {
    //Movement
    public static readonly string LEFT = "left";
    public static readonly string RIGHT = "right";
    public static readonly string UP = "up";
    public static readonly string DOWN = "down";

    public static readonly string JUMP = "jump";
    public static readonly string RUN = "run";

    //Mouse Input
    public static readonly string ZOOMIN = "zoomIn";
    public static readonly string ZOOMOUT = "zoomOut";
    public static readonly string ATTACK_BASE = "attack";

    //Interaction
    public static readonly string INTERACT = "interact";
    public static readonly string ALT_INTERACT = "altInteract";

    //Misc Input
    public static readonly string QUIT = "quit";
    public static readonly string UNSTICK = "unstick";
    public static readonly string PAUSE = "pause";

    //Saving/Loading
    public static readonly string SAVEABLE = "saveable";

}
