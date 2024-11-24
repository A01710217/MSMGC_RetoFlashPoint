using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Node {
    public int[] id;
    public string type;
    public int value;
    public string status;
}

[System.Serializable]
public class Edge {
    public int[] source;
    public int[] target;
    public int weight;
    public string category;
    public int life;
}

[System.Serializable]
public class AgentState {
    public int agent_id;
    public int[] current_node;
    public int[] neighbor;
    public string action;
    public int[] direction;
    public bool carrying;
}

[System.Serializable]
public class InitialConfig {
    public List<AgentState> initialAgents;
    public MazeGraph initialModel;
}

[System.Serializable]
public class MazeGraph {
    public List<Node> nodes;
    public List<Edge> edges;
}

public class UnityClient : MonoBehaviour {
    public HouseBuilder houseBuilder;  // Referencia al script HouseBuilder
    public GameObject agentPrefab;    // Prefab para los agentes

    void Start() {
        StartCoroutine(DownloadInitialConfig());
    }

    IEnumerator DownloadInitialConfig() {
        // Realizar solicitud POST al servidor para obtener la configuración inicial
        UnityWebRequest request = UnityWebRequest.PostWwwForm("http://localhost:8585", "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            Debug.Log("Response: " + request.downloadHandler.text);

            // Parsear el JSON recibido
            InitialConfig initialConfig = JsonUtility.FromJson<InitialConfig>(request.downloadHandler.text);
            Debug.Log("Nodos recibidos: " + initialConfig.initialModel.nodes.Count);
            Debug.Log("Aristas recibidas: " + initialConfig.initialModel.edges.Count);

            // Construir la casa con el grafo recibido
            houseBuilder.BuildHouse(initialConfig.initialModel, initialConfig.initialAgents, agentPrefab);
        } else {
            Debug.Log("Error: " + request.error);
        }
    }
}
