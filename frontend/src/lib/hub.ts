import * as signalR from '@microsoft/signalr'
import type { Message } from '../types';
import { useMessageStore } from '../stores/messageStore';

let connection: signalR.HubConnection | null = null;

const connectSignalR = async () => {
  const {addMessage} = useMessageStore()

  if (connection) return connection;

  connection = new signalR.HubConnectionBuilder().withUrl("/api/hub", {withCredentials: true}).withAutomaticReconnect().build()

  connection.on("ReceiveMessage", (message: Message) => {
    addMessage(message);
    //update conversation list as well
  })

  await connection.start();
  console.log("Connected to hub")
  return connection;
}

const getSignalR = () => connection;

const disconnectSignalR = async () => {
  if (connection) {
    await connection.stop()
    console.log("Disconnected from hub")
    connection = null;
  }
}


export {connectSignalR, getSignalR, disconnectSignalR}