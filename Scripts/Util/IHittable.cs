using Godot;
using System;

//IHittable is for all things that can be hit including trees, ore and later enemies
public interface IHittable {
    public void Hit(Node3D body);
}
