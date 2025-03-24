using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridPropertiesManager : MonoBehaviour
{
    private static GridPropertiesManager _instance;

    // Public property to access the instance
    public static GridPropertiesManager Instance => _instance;
    public Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDictionary;
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;

    private void Awake()
    {
        // If an instance already exists and it's not this one
        if (_instance != null && _instance != this)
        {
            // Destroy this instance
            Destroy(gameObject);
            return;
        }
        
        // Set this as the current instance
        _instance = this;
    }

    private void Start()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        InitialiseGridProperties();
    }


private void InitialiseGridProperties()
{
    
     string activeSceneName = SceneManager.GetActiveScene().name;
    Debug.Log($"🔍 Active scene: {activeSceneName}");

    foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
    {
        Debug.Log($"📝 Checking grid properties for: {so_GridProperties.sceneName}");

        if (!so_GridProperties.sceneName.ToString().Equals(activeSceneName, StringComparison.OrdinalIgnoreCase))
            continue;

        Debug.Log($"✅ Loading grid properties for scene: {activeSceneName}");
        
    }

    foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
    {
        if (!activeSceneName.Contains(so_GridProperties.sceneName.ToString(), StringComparison.OrdinalIgnoreCase))

            continue;

        Debug.Log($"✅ Loading grid properties for scene: {activeSceneName}");

        Dictionary<string, GridPropertyDetails> propertyDict = new Dictionary<string, GridPropertyDetails>();

        foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
        {
            GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(
                gridProperty.gridCoordinate.x,
                gridProperty.gridCoordinate.y,
                propertyDict);

            if (gridPropertyDetails == null)
                gridPropertyDetails = new GridPropertyDetails();

            switch (gridProperty.gridBoolProperty)
            {
                case GridBoolProperty.diggable:
                    gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                    break;
                case GridBoolProperty.canDropItem:
                    gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                    break;
                case GridBoolProperty.canPlaceFurniture:
                    gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                    break;
                case GridBoolProperty.isPath:
                    gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                    break;
                case GridBoolProperty.isNPCObstacle:
                    gridPropertyDetails.isNPCObstacle = gridProperty.gridBoolValue;
                    break;
            }

            SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y, gridPropertyDetails, propertyDict);
        }

        this.gridPropertyDictionary = propertyDict;
        return;
    }

    Debug.LogError($"❌ No matching SO_GridProperties found for active scene: {activeSceneName}");
}

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        string key = "x" + gridX + "y" + gridY;

        GridPropertyDetails gridPropertyDetails;

        // Check if grid property details exist for coordinate and retrieve
        if (gridPropertyDictionary == null)
        {
            Debug.LogError("gridPropertyDictionary is NULL in GetGridPropertyDetails()");
            return null;
        }
        if(!gridPropertyDictionary.TryGetValue(key, out gridPropertyDetails))
        {
            // if not found
            return null;    
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY, gridPropertyDictionary);
    }
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails, gridPropertyDictionary);
    }
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails, Dictionary<string, GridPropertyDetails> gridPropertyDictionary)
    {
        // Construct key from coordinate
        string key = "x" + gridX + "y" + gridY;

        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;

        // Set value
        gridPropertyDictionary[key] = gridPropertyDetails;
    }

}