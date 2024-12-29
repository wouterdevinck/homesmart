import axios from 'axios'
import * as signalR from '@microsoft/signalr'
const url = import.meta.env.VITE_API_URL

export default {

  getRooms(cb) {
    axios.get(`${url}/api/v1/rooms`).then((response) => {
      cb(response.data)
    })
  },

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
    connection.on("deviceupdates", device => {
      console.log(`Device ${device.deviceId} update recieved`)
      console.log(device)
      cb(device)
    })
    start()
  },

  sendDeviceCommand(deviceId, command, payload = {}, error = undefined) {
    axios.post(`${url}/api/v1/devices/${deviceId}/commands/${command}`, payload).catch(function (ex) {
      console.log(ex)
      if(error) error()
    })
  },

  getMetadata(cb) {
    axios.get(`${url}/api/v1/telemetry/metadata`).then((response) => {
      cb(response.data)
    })
  },

  getData(cb, device, point, diffWindow, meanWindow, since, toAgo, from, to) {
    var dataurl = new URL(`${url}/api/v1/devices/${device}/data/${point}`)
    if(diffWindow) dataurl.searchParams.append('diffWindow', diffWindow)
    if(meanWindow) dataurl.searchParams.append('meanWindow', meanWindow)
    if(since) dataurl.searchParams.append('since', since)
    if(toAgo) dataurl.searchParams.append('toAgo', toAgo)
    if(from) dataurl.searchParams.append('from', from)
    if(to) dataurl.searchParams.append('to', to)
    axios.get(dataurl.toString()).then((response) => {
      cb(response.data)
    })
  }

}