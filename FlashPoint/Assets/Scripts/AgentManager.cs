using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentManager : MonoBehaviour {
    // Prefabs para los agentes
    public GameObject[] agentPrefabs;
    // Prefab para mostrar que el agente está cargando
    public GameObject carryingIndicatorPrefab;

    private List<GameObject> agents = new List<GameObject>();
    private Dictionary<int, GameObject> carryingIndicators = new Dictionary<int, GameObject>();

    // Método para instanciar agentes en la escena
    public void CreateAgents(List<AgentState> initialAgents) {
        if (agentPrefabs == null || agentPrefabs.Length == 0) {
            Debug.LogError("No hay prefabs de agentes asignados.");
            return;
        }

        foreach (var agentState in initialAgents) {
            GameObject randomPrefab = agentPrefabs[Random.Range(0, agentPrefabs.Length)];
            Vector3 agentPosition = new Vector3(agentState.current_node[1], 0f, agentState.current_node[0]);

            GameObject newAgent = Instantiate(randomPrefab, agentPosition, Quaternion.identity);
            newAgent.name = $"Agent_{agentState.agent_id}";
            agents.Add(newAgent);

            // Si el agente está cargando, agregar el indicador
            if (agentState.carrying) {
                AddCarryingIndicator(newAgent, agentState.agent_id);
            }
        }
    }

    // Método para actualizar los estados de los agentes
    public void UpdateAgentStates(List<AgentState> agentStates) {
        foreach (var state in agentStates) {
            GameObject agent = agents.Find(a => a.name == $"Agent_{state.agent_id}");
            if (agent != null) {
                // Calcular la nueva posición del agente
                Vector3 newPosition = new Vector3(state.current_node[1], 0f, state.current_node[0]);

                // Calcular la dirección hacia la celda vecina
                Vector3 direction = new Vector3(state.neighbor[1], 0f, state.neighbor[0]) - newPosition;

                // Mover al agente y luego rotarlo hacia la celda vecina
                StartCoroutine(MoveAndRotateAgent(agent, newPosition, direction, 2f, state));

                // Validar que el agente ya esté sobre la celda objetivo antes de actualizar el indicador
                //UpdateCarryingIndicator(agent, state.agent_id, state.carrying);
            }
        }
    }

    // Corutina para mover y luego rotar al agente
    private IEnumerator MoveAndRotateAgent(GameObject agent, Vector3 targetPosition, Vector3 direction, float duration, AgentState state) {
        Vector3 startPosition = agent.transform.position;
        Quaternion startRotation = agent.transform.rotation;

        float elapsedTime = 0f;

        // Movimiento suave hacia la posición objetivo
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            agent.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null; // Esperar al siguiente frame
        }

        // Asegurarse de que el agente termine exactamente en la posición objetivo
        agent.transform.position = targetPosition;

        // Realizar la rotación hacia la dirección vecina
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            elapsedTime = 0f;
            while (elapsedTime < duration / 2) { // La rotación será más rápida (mitad del tiempo)
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / (duration / 2);

                agent.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

                yield return null; // Esperar al siguiente frame
            }

            // Asegurarse de que el agente termine con la rotación objetivo
            agent.transform.rotation = targetRotation;
        }

        // Validar si el agente llegó a la posición `current_node`
        if (Vector3.Distance(agent.transform.position, targetPosition) < 0.1f) {
            Debug.Log($"Agent {state.agent_id} reached position {targetPosition}");

            // Actualizar el indicador de carga si el estado `carrying` cambió
            UpdateCarryingIndicator(agent, state.agent_id, state.carrying);
        }
    }

    // Método para agregar un indicador de carga al agente
    private void AddCarryingIndicator(GameObject agent, int agentId) {
        if (carryingIndicatorPrefab != null && !carryingIndicators.ContainsKey(agentId)) {
            GameObject indicator = Instantiate(carryingIndicatorPrefab, agent.transform);
            indicator.transform.localPosition = new Vector3(0, 1.5f, 0); // Ajustar la posición del indicador encima del agente
            carryingIndicators[agentId] = indicator;
        }
    }

    // Método para eliminar el indicador de carga del agente
    private void RemoveCarryingIndicator(int agentId) {
        if (carryingIndicators.ContainsKey(agentId)) {
            Destroy(carryingIndicators[agentId]);
            carryingIndicators.Remove(agentId);
        }
    }

    // Método para actualizar el indicador de carga
    private void UpdateCarryingIndicator(GameObject agent, int agentId, bool isCarrying) {
        if (isCarrying) {
            AddCarryingIndicator(agent, agentId);
        } else {
            RemoveCarryingIndicator(agentId);
        }
    }
}
