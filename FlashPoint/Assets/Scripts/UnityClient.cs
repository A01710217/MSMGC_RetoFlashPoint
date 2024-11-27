using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

// Clases para manejar la deserialización del JSON

[System.Serializable]
public class Node {
    public int[] id;
    public string type;
    public string status;  // Aseguramos que sea string para manejar valores vacíos
}

[System.Serializable]
public class Edge {
    public int[] source;
    public int[] target;
    public int weight;
    public string category;
    public int life;
    public string status;
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
public class Metadata {
    public int damage_points;
    public int deaths;
    public int saved_victims;
    public int steps;
}

[System.Serializable]
public class MazeGraph {
    public List<Node> nodes;
    public List<Edge> edges;
    public Metadata metadata;
}

[System.Serializable]
public class InitialConfig {
    public List<AgentState> initialAgents;
    public MazeGraph initialModel;
}

[System.Serializable]
public class AnimationStep {
    public int step;
    public List<AgentState> animationAgent;
    public MazeGraph animationModel;
}

[System.Serializable]
public class CombinedConfigAnimation {
    public InitialConfig initialConfig;
    public List<AnimationStep> animationData;
}

public class UnityClient : MonoBehaviour {
    public HouseBuilder houseBuilder;  // Referencia al script HouseBuilder
    public AgentManager agentManager;  // Referencia al script AgentManager
    public MetadataUIController metadataUIController;  // Referencia al script MetadataUIController
    private int currentStep = 0;
    private List<AnimationStep> animationSteps;
    public SceneController sceneController;  // Referencia al script SceneController


    void Start() {
        StartCoroutine(DownloadCombinedConfig());
    }

    IEnumerator DownloadCombinedConfig() {
        // Realizar solicitud POST al servidor para obtener el JSON combinado
        UnityWebRequest request = UnityWebRequest.PostWwwForm("http://localhost:8585", "");
        yield return request.SendWebRequest(); // Esperar a que se complete la solicitud

        if (request.result == UnityWebRequest.Result.Success) {
            Debug.Log("Response: " + request.downloadHandler.text);

            try {
                // Parsear el JSON combinado
                CombinedConfigAnimation combinedData = JsonUtility.FromJson<CombinedConfigAnimation>(request.downloadHandler.text);

                // Debugear: Verificar si la deserialización fue exitosa
                Debug.Log("Initial Agents: " + combinedData.initialConfig.initialAgents.Count);
                Debug.Log("Initial Model Nodes: " + combinedData.initialConfig.initialModel.nodes.Count);
                Debug.Log("Initial Model Edges: " + combinedData.initialConfig.initialModel.edges.Count);

                // Construir la casa con el grafo recibido
                houseBuilder.BuildHouse(combinedData.initialConfig.initialModel);

                // Crear los agentes
                agentManager.CreateAgents(combinedData.initialConfig.initialAgents);

                // Guardar los pasos de animación
                animationSteps = combinedData.animationData;
                StartCoroutine(PlayAnimationSteps());
            } catch (Exception e) {
                Debug.LogError("Error al deserializar el JSON: " + e.Message);
                yield break; // Finalizar la corutina en caso de error
            }
        } else {
            Debug.Log("Error: " + request.error);
            yield break; // Finalizar la corutina en caso de error en la solicitud
        }
    }


     // Corutina para ejecutar los pasos de animación
    IEnumerator PlayAnimationSteps() {
        while (currentStep < animationSteps.Count) {
            Debug.Log($"Playing Animation Step {animationSteps[currentStep].step}");

            // Actualizar la casa después de que los agentes hayan terminado
            houseBuilder.BuildHouse(animationSteps[currentStep].animationModel);
            // Esperar a que los agentes terminen de moverse
            yield return StartCoroutine(agentManager.UpdateAgentStatesAndWait(animationSteps[currentStep].animationAgent, 0.5f));

            // Actualizar la UI con los metadatos
            metadataUIController.UpdateMetadata(animationSteps[currentStep].animationModel.metadata);
            // Incrementar el paso actual
            currentStep++;

            if (currentStep >= animationSteps.Count) {
                Debug.Log("Animation completed.");
                // Validar si se gano
                if (animationSteps[currentStep - 1].animationModel.metadata.saved_victims >= 6) {
                    // Cargar la escena de Juego Ganado
                    sceneController.LoadWinScene();
                } else {
                    // Cargar la escena de Juego Perdido
                    sceneController.LoadGameOverScene();
                }
                break;
            }
        }
    }
}