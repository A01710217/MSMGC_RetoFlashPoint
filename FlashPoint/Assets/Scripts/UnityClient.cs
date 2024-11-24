using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;


[System.Serializable]
public class Node {
    public int[] id;
    public string type;
    public string status; // Cambié a string para que acepte valores vacíos
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
    public AgentManager agentManager;  // Referencia al script AgentManager

    void Start() {
        StartCoroutine(DownloadInitialConfig());
    }

    IEnumerator DownloadInitialConfig() {
        // Realizar solicitud POST al servidor para obtener la configuración inicial
        UnityWebRequest request = UnityWebRequest.PostWwwForm("http://localhost:8585", "");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            Debug.Log("Response: " + request.downloadHandler.text);

            try {
                // Parsear el JSON recibido
                InitialConfig initialConfig = JsonUtility.FromJson<InitialConfig>(request.downloadHandler.text);

                // Debugear: Verificar si la deserialización fue exitosa
                Debug.Log("Initial Agents: " + initialConfig.initialAgents.Count);
                Debug.Log("Initial Model Nodes: " + initialConfig.initialModel.nodes.Count);
                Debug.Log("Initial Model Edges: " + initialConfig.initialModel.edges.Count);

                // Construir la casa con el grafo recibido
                houseBuilder.BuildHouse(initialConfig.initialModel);

                // Crear los agentes
                agentManager.CreateAgents(initialConfig.initialAgents);
            } catch (Exception e) {
                Debug.LogError("Error al deserializar el JSON: " + e.Message);
            }
        } else {
            Debug.Log("Error: " + request.error);
        }
    }
}
