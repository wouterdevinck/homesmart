export default {
  methods: {
    getDeviceIcon: function(device) {
      switch(device.type) {
        case "light":
          return "lightbulb"
        case "switch":
          return "toggles"
        case "outlet":
          return "outlet"
        case "temperature":
          return "thermometer-half"
        case "leak":
          return "droplet"
        case "hub":
          return "hdd-network"
        case "solar":
          return "sun"
        case "fancoil":
          return "fire"
        case "trv":
          return "fire"
        case "shutter":
          return "list"
        case "camera":
          return "camera-video"
        case "wifi":
          return "wifi"
        case "networkswitch":
          return "ethernet"
        case "watermeter":
          return "droplet"
        default:
          return "hdd"
      }
    },
    getRoomIcon: function(room) {
      return `room-${room.type}`
    }
  }
}