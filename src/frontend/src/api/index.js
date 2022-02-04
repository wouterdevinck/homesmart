import axios from 'axios'
import * as signalR from '@microsoft/signalr'

// const url = "http://192.168.1.180:5000"
const url = ""

export default {

  getDevices(cb) {
    axios.get(`${url}/api/v1/devices`).then((response) => {
      cb(response.data)
    })
  },

  getAutomations(cb) {
    axios.get(`${url}/api/v1/automations`).then((response) => {
      cb(response.data)
    })
  },

  subscribeTwinUpdates(cb) {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${url}/api/v1/`)
      .configureLogging(signalR.LogLevel.Information)
      .build();
    async function start() {
      try {
        await connection.start()
        console.log("Connected")
      } catch (err) {
        console.log(err)
        setTimeout(start, 5000)
      }
    }
    connection.onclose(start)
    connection.on("twinupdates", device => {
      console.log(`Device ${device.deviceId} twin update recieved`)
      cb(device)
    })
    start()
  },

  sendDeviceCommand(deviceId, command) {
    axios.post(`${url}/api/v1/devices/${deviceId}/commands/${command}`, {})
  }

}