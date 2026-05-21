using Godot;

//NavigationPaths sets up two navigation paths for different agent types
public partial class NavigationPaths : Node3D {

    private static NavigationPaths instance;
    public static NavigationPaths Instance { get => instance; }

    [Export] private float npcAgentHeight = 1.5f;
    [Export] private float npcAgentRadius = 0.5f;
    [Export] private float driveableAgentHeight = 1.5f;
    [Export] private float driveableAgentRadius = 3f;

    Rid mapNPC;
    Rid mapDriveable;

    public Rid GetNavigationMapNPC { get => mapNPC; }
    public Rid GetNavigationMapDriveable { get => mapDriveable; }

    [Signal] public delegate void OnNavigationMapCreatedEventHandler();

    public override void _Ready() {
        instance = this;
        CallDeferred(MethodName.CustomSetup);
    }

    /*
    public async void MakeRegions() {

        float cellSize = .265f;
        float cellHeight = .265f;

        //navigation mesh for each actor size
        NavigationMesh navigationMeshNPC = new NavigationMesh();
        NavigationMesh navigationMeshCart = new NavigationMesh();

        //set appropriate agent parameters
        navigationMeshNPC.AgentHeight = npcAgentHeight;
        navigationMeshNPC.AgentRadius = npcAgentRadius;
        navigationMeshCart.AgentHeight = cartAgentHeight;
        navigationMeshCart.AgentRadius = cartAgentRadius;

        navigationMeshNPC.CellSize = cellSize;
        navigationMeshNPC.CellHeight = cellHeight;
        navigationMeshCart.CellSize = cellSize;
        navigationMeshCart.CellHeight = cellHeight;

        GD.PrintS("--------------cellsize:", navigationMeshNPC.CellSize);

        //create source geometery to hold parsed geo data
        NavigationMeshSourceGeometryData3D sourceGeometryData = new NavigationMeshSourceGeometryData3D();

        // Parse the source geometry from the scene tree on the main thread.
        // The navigation mesh is only required for the parse settings so any of the three will do.
        NavigationServer3D.ParseSourceGeometryData(navigationMeshNPC, sourceGeometryData, this);

        NavigationServer3D.BakeFromSourceGeometryData(navigationMeshNPC, sourceGeometryData);
        NavigationServer3D.BakeFromSourceGeometryData(navigationMeshCart, sourceGeometryData);

        //Create different navigation maps on the NavigationServer
        navigationMapNPC = NavigationServer3D.MapCreate();
        navigationMapCart = NavigationServer3D.MapCreate();

        NavigationServer3D.MapSetCellSize(navigationMapNPC, cellSize);
        NavigationServer3D.MapSetCellSize(navigationMapCart, cellSize);
        NavigationServer3D.MapSetCellHeight(navigationMapNPC, cellHeight);
        NavigationServer3D.MapSetCellHeight(navigationMapCart, cellHeight);


        //set new navigation maps as active
        NavigationServer3D.MapSetActive(navigationMapNPC, true);
        NavigationServer3D.MapSetActive(navigationMapCart, true);

        //Create a region for each map
        Rid navigationRegionNPC = NavigationServer3D.RegionCreate();
        Rid navigationRegionCart = NavigationServer3D.RegionCreate();

        //Add regions to the maps
        NavigationServer3D.RegionSetMap(navigationRegionNPC, navigationMapNPC);
        NavigationServer3D.RegionSetMap(navigationRegionCart, navigationMapCart);

        //set navigation mesh for each region
        NavigationServer3D.RegionSetNavigationMesh(navigationRegionNPC, navigationMeshNPC);
        NavigationServer3D.RegionSetNavigationMesh(navigationRegionCart, navigationMeshCart);

        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        EmitSignal(SignalName.OnNavigationMapCreated);

    }
*/

    //Sets up the Navigation mesh 
    private async void CustomSetup() {
        BakeNavMesh();

        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Query the path from the navigation server.
        var startPosition = new Vector3(1.8f, 1.5f, -15.3f);
        var targetPosition = new Vector3(0.8906102f, 0.98529f, -22.2682f);

        Vector3[] path = NavigationServer3D.MapGetPath(mapNPC, startPosition, targetPosition, optimize: true);

        Vector3[] pathDriveable = NavigationServer3D.MapGetPath(mapDriveable, startPosition, targetPosition, optimize: true);



        GD.Print("Found a path!");
        GD.PrintS("npc: ", (Variant)path, "Driveable: ", (Variant)pathDriveable);
    }

    //Makes two navigation meshes, one for NPCs and the other for Driveables
    public void BakeNavMesh() {
        float cellSize = .27f;
        float cellHeight = .27f;

        //navigation mesh for each actor size
        NavigationMesh navmeshNPC = new NavigationMesh {
            //set appropriate agent parameters
            AgentHeight = npcAgentHeight,
            AgentRadius = npcAgentRadius,
            CellHeight = cellHeight,
            CellSize = cellSize
        };

        NavigationMesh navmeshDriveable = new NavigationMesh {
            //set appropriate agent parameters
            AgentHeight = driveableAgentHeight,
            AgentRadius = driveableAgentRadius,
            CellHeight = cellHeight,
            CellSize = cellSize
        };



        //create source geometery to hold parsed geo data
        NavigationMeshSourceGeometryData3D sourceGeometryData = new NavigationMeshSourceGeometryData3D();

        // Parse the source geometry from the scene tree on the main thread.
        // The navigation mesh is only required for the parse settings so any of the three will do.
        NavigationServer3D.ParseSourceGeometryData(navmeshNPC, sourceGeometryData, this);

        NavigationServer3D.BakeFromSourceGeometryData(navmeshNPC, sourceGeometryData);
        NavigationServer3D.BakeFromSourceGeometryData(navmeshDriveable, sourceGeometryData);

        mapNPC = NavigationServer3D.MapCreate();
        NavigationServer3D.MapSetActive(mapNPC, true);
        mapDriveable = NavigationServer3D.MapCreate();
        NavigationServer3D.MapSetActive(mapDriveable, true);


        NavigationServer3D.MapSetCellHeight(mapNPC, cellHeight);
        NavigationServer3D.MapSetCellSize(mapNPC, cellSize);
        NavigationServer3D.MapSetCellHeight(mapDriveable, cellHeight);
        NavigationServer3D.MapSetCellSize(mapDriveable, cellSize);

        Rid regionNPC = NavigationServer3D.RegionCreate();
        NavigationServer3D.RegionSetMap(regionNPC, mapNPC);

        Rid regionDriveable = NavigationServer3D.RegionCreate();
        NavigationServer3D.RegionSetMap(regionDriveable, mapDriveable);

        NavigationServer3D.RegionSetNavigationMesh(regionNPC, navmeshNPC);
        NavigationServer3D.RegionSetNavigationMesh(regionDriveable, navmeshDriveable);
    }
}
