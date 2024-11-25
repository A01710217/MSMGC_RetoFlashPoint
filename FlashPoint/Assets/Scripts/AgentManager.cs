using UnityEngine;
using System.Collections.Generic;

public class AgentManager : MonoBehaviour {
    // Prefabs para los agentes
    public GameObject[] agentPrefabs;

    private List<GameObject> agents = new List<GameObject>();

    // Método para instanciar agentes en la escena
    public void CreateAgents(List<AgentState> initialAgents) {
        // Verificar que haya prefabs disponibles
        if (agentPrefabs == null || agentPrefabs.Length == 0) {
            Debug.LogError("No hay prefabs de agentes asignados.");
            return;
        }

        // Crear un agente por cada estado inicial
        foreach (var agentState in initialAgents) {
            // Seleccionar un prefab aleatorio
            GameObject randomPrefab = agentPrefabs[Random.Range(0, agentPrefabs.Length)];

            Vector3 agentPosition = new Vector3(agentState.current_node[1], 0f, agentState.current_node[0]);

            // Calcular la dirección hacia la celda vecina
            Vector3 direction = new Vector3(agentState.neighbor[1], 0f, agentState.neighbor[0]) - agentPosition;

            // Calcular la rotación necesaria para mirar hacia la celda vecina
            Quaternion rotation = Quaternion.LookRotation(direction);
            rotation.x = 0f; // No rotar en el eje X
            rotation.z = 0f; // No rotar en el eje Z

            // Instanciar el agente con el prefab seleccionado
            GameObject newAgent = Instantiate(randomPrefab, agentPosition, rotation);
            newAgent.name = $"Agent_{agentState.agent_id}";
            agents.Add(newAgent);
        }
    }

    // Método para actualizar los estados de los agentes
    public void UpdateAgentStates(List<AgentState> agentStates) {
        foreach (var state in agentStates) {
            GameObject agent = agents.Find(a => a.name == $"Agent_{state.agent_id}");
            if (agent != null) {
                // Actualizar la posición del agente
                Vector3 newPosition = new Vector3(state.current_node[1], 0f, state.current_node[0]);
                agent.transform.position = newPosition;

                // Rotar el agente hacia la celda vecina
                RotateAgentTowardsNeighbor(agent, state);
            }
        }
    }

    // Método para rotar el agente hacia la celda a realizar la acción
    private void RotateAgentTowardsNeighbor(GameObject agent, AgentState state) {
        // Comprobar que 'neighbor' tiene al menos dos valores
        if (state.neighbor.Length < 2) {
            Debug.LogWarning($"Invalid neighbor data for agent {agent.name}. Neighbor data: {string.Join(", ", state.neighbor)}");
            return;
        }

        // Vector que representa la dirección de la celda a realizar la acción
        Vector3 direction = new Vector3(state.neighbor[1], 0f, state.neighbor[0]) - agent.transform.position;

        // Comprobar si la dirección es válida (no es el mismo punto)
        if (direction != Vector3.zero) {
            // Calcular la rotación necesaria para mirar hacia la celda a realizar la acción
            Quaternion rotation = Quaternion.LookRotation(direction);
            rotation.x = 0f; // No rotar en el eje X
            rotation.z = 0f; // No rotar en el eje Z

            // Aplicar la rotación solo sobre el eje Y
            agent.transform.rotation = rotation;

            // Depurar la rotación (mostrar en la consola)
            Debug.Log($"Agent {agent.name} rotation: {agent.transform.rotation.eulerAngles}");
        } else {
            Debug.LogWarning($"Agent {agent.name} is already facing the target.");
        }
    }
}
