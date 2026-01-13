import * as signalR from "@microsoft/signalr";
import { ref, onUnmounted } from "vue";

const HUB_URL = "http://localhost:5041/gamehub";

/**
 * Vue 3 composable for SignalR connection to GameHub
 * Supports both Dashboard (master) and Overlay (viewer) roles
 */
export function useSignalR() {
    const connection = ref(null);
    const isConnected = ref(false);
    const lastEvent = ref(null);
    const role = ref(null); // 'master' or 'viewer'

    let eventCallbacks = [];
    let gameStateCallbacks = [];
    let damageCallbacks = [];
    let buffCallbacks = [];

    /**
     * Connect to the SignalR hub and join the broadcaster's group
     * @param {string} twitchId - The broadcaster's Twitch ID
     * @param {string} connectionRole - 'master' (dashboard) or 'viewer' (overlay)
     */
    async function connect(twitchId, connectionRole = 'viewer') {
        if (connection.value) {
            console.warn("SignalR already connected");
            return;
        }

        role.value = connectionRole;

        connection.value = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL)
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // Handle connection state changes
        connection.value.onreconnecting(() => {
            console.log("SignalR reconnecting...");
            isConnected.value = false;
        });

        connection.value.onreconnected(() => {
            console.log("SignalR reconnected");
            isConnected.value = true;
            // Rejoin game group after reconnect
            connection.value.invoke("JoinGame", twitchId, role.value);
        });

        connection.value.onclose(() => {
            console.log("SignalR disconnected");
            isConnected.value = false;
        });

        // Listen for the Joined confirmation
        connection.value.on("Joined", (data) => {
            console.log("Joined game hub:", data);
        });

        // Listen for Twitch events from backend
        connection.value.on("TwitchEvent", (event) => {
            console.log("TwitchEvent received:", event);
            lastEvent.value = event;
            eventCallbacks.forEach(callback => callback(event));
        });

        // Listen for game state updates (for Overlay)
        connection.value.on("GameStateUpdate", (gameState) => {
            gameStateCallbacks.forEach(callback => callback(gameState));
        });

        // Listen for damage numbers (for Overlay)
        connection.value.on("DamageDealt", (damageData) => {
            damageCallbacks.forEach(callback => callback(damageData));
        });

        // Listen for buff notifications (for Overlay)
        connection.value.on("BuffApplied", (buffData) => {
            buffCallbacks.forEach(callback => callback(buffData));
        });

        try {
            await connection.value.start();
            console.log("SignalR connected to", HUB_URL, "as", connectionRole);
            isConnected.value = true;

            // Join the broadcaster's group with role
            await connection.value.invoke("JoinGame", twitchId, connectionRole);
        } catch (err) {
            console.error("SignalR connection failed:", err);
            isConnected.value = false;
            throw err;
        }
    }

    /**
     * Broadcast game state to all overlays (called by Dashboard)
     */
    async function broadcastGameState(twitchId, gameState) {
        if (!connection.value || !isConnected.value) return;
        try {
            await connection.value.invoke("BroadcastGameState", twitchId, gameState);
        } catch (err) {
            console.warn("Failed to broadcast game state:", err.message);
        }
    }

    /**
     * Broadcast damage number (called by Dashboard)
     */
    async function broadcastDamage(twitchId, damageData) {
        if (!connection.value || !isConnected.value) return;
        try {
            await connection.value.invoke("BroadcastDamage", twitchId, damageData);
        } catch (err) {
            console.warn("Failed to broadcast damage:", err.message);
        }
    }

    /**
     * Broadcast buff notification (called by Dashboard)
     */
    async function broadcastBuff(twitchId, buffData) {
        if (!connection.value || !isConnected.value) return;
        try {
            await connection.value.invoke("BroadcastBuff", twitchId, buffData);
        } catch (err) {
            console.warn("Failed to broadcast buff:", err.message);
        }
    }

    /**
     * Disconnect from the SignalR hub
     */
    async function disconnect(twitchId) {
        if (!connection.value) return;

        try {
            if (twitchId) {
                await connection.value.invoke("LeaveGame", twitchId);
            }
            await connection.value.stop();
            console.log("SignalR disconnected");
        } catch (err) {
            console.error("SignalR disconnect error:", err);
        } finally {
            connection.value = null;
            isConnected.value = false;
        }
    }

    /**
     * Register a callback for Twitch events
     */
    function onTwitchEvent(callback) {
        eventCallbacks.push(callback);
        return () => {
            eventCallbacks = eventCallbacks.filter(cb => cb !== callback);
        };
    }

    /**
     * Register a callback for game state updates (Overlay uses this)
     */
    function onGameStateUpdate(callback) {
        gameStateCallbacks.push(callback);
        return () => {
            gameStateCallbacks = gameStateCallbacks.filter(cb => cb !== callback);
        };
    }

    /**
     * Register a callback for damage events (Overlay uses this)
     */
    function onDamageDealt(callback) {
        damageCallbacks.push(callback);
        return () => {
            damageCallbacks = damageCallbacks.filter(cb => cb !== callback);
        };
    }

    /**
     * Register a callback for buff events (Overlay uses this)
     */
    function onBuffApplied(callback) {
        buffCallbacks.push(callback);
        return () => {
            buffCallbacks = buffCallbacks.filter(cb => cb !== callback);
        };
    }

    // Cleanup on component unmount
    onUnmounted(() => {
        if (connection.value) {
            connection.value.stop();
        }
    });

    return {
        connection,
        isConnected,
        lastEvent,
        role,
        connect,
        disconnect,
        onTwitchEvent,
        // New broadcast methods (for Dashboard)
        broadcastGameState,
        broadcastDamage,
        broadcastBuff,
        // New listener methods (for Overlay)
        onGameStateUpdate,
        onDamageDealt,
        onBuffApplied
    };
}

