import * as signalR from "@microsoft/signalr";
import { ref, onUnmounted } from "vue";

const HUB_URL = "http://localhost:5041/gamehub";

/**
 * Vue 3 composable for SignalR connection to GameHub
 * Handles connection, group joining, and Twitch event callbacks
 */
export function useSignalR() {
    const connection = ref(null);
    const isConnected = ref(false);
    const lastEvent = ref(null);

    let eventCallbacks = [];

    /**
     * Connect to the SignalR hub and join the broadcaster's group
     * @param {string} twitchId - The broadcaster's Twitch ID
     */
    async function connect(twitchId) {
        if (connection.value) {
            console.warn("SignalR already connected");
            return;
        }

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
            connection.value.invoke("JoinGame", twitchId);
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

            // Call all registered callbacks
            eventCallbacks.forEach(callback => callback(event));
        });

        try {
            await connection.value.start();
            console.log("SignalR connected to", HUB_URL);
            isConnected.value = true;

            // Join the broadcaster's group
            await connection.value.invoke("JoinGame", twitchId);
        } catch (err) {
            console.error("SignalR connection failed:", err);
            isConnected.value = false;
            throw err;
        }
    }

    /**
     * Disconnect from the SignalR hub
     * @param {string} twitchId - The broadcaster's Twitch ID (for leaving group)
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
     * @param {Function} callback - Function to call when event is received
     * @returns {Function} Unsubscribe function
     */
    function onTwitchEvent(callback) {
        eventCallbacks.push(callback);

        // Return unsubscribe function
        return () => {
            eventCallbacks = eventCallbacks.filter(cb => cb !== callback);
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
        connect,
        disconnect,
        onTwitchEvent
    };
}
