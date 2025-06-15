using Godot;
using System;

//deprecated class that will be deleted soon
public partial class NavAgain : Node3D {

    public override void _Ready() {
        CallDeferred(MethodName.CustomSetup);
    }

    private async void CustomSetup() {
        Rid mapNPC = BakeNavMesh();

        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Query the path from the navigation server.
        var startPosition = new Vector3(0.1f, 0.0f, 0.1f);
        var targetPosition = new Vector3(1.0f, 0.0f, 1.0f);

        Vector3[] path = NavigationServer3D.MapGetPath(mapNPC, startPosition, targetPosition, optimize: true);

        GD.Print("Found a path!");
        GD.Print((Variant)path);

    }

    public Rid BakeNavMesh() {
        float cellSize = .3f;
        float cellHeight = .3f;

        //navigation mesh for each actor size
        NavigationMesh navmeshNPC = new NavigationMesh {
            //set appropriate agent parameters
            AgentHeight = 1.5f,
            AgentRadius = 0.5f,
            CellHeight = cellHeight,
            CellSize = cellSize
        };

        //create source geometery to hold parsed geo data
        NavigationMeshSourceGeometryData3D sourceGeometryData = new NavigationMeshSourceGeometryData3D();

        // Parse the source geometry from the scene tree on the main thread.
        // The navigation mesh is only required for the parse settings so any of the three will do.
        NavigationServer3D.ParseSourceGeometryData(navmeshNPC, sourceGeometryData, this);

        NavigationServer3D.BakeFromSourceGeometryData(navmeshNPC, sourceGeometryData);

        Rid mapNPC = NavigationServer3D.MapCreate();
        NavigationServer3D.MapSetActive(mapNPC, true);

        NavigationServer3D.MapSetCellHeight(mapNPC, cellHeight);
        NavigationServer3D.MapSetCellSize(mapNPC, cellSize);

        Rid regionNPC = NavigationServer3D.RegionCreate();
        NavigationServer3D.RegionSetMap(regionNPC, mapNPC);

        NavigationServer3D.RegionSetNavigationMesh(regionNPC, navmeshNPC);
        return mapNPC;
    }
}
