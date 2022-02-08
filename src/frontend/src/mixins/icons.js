export default {
  methods: {
    getDeviceIcon: function(device) {
      switch(device.type) {
        case "media":
          return "film"
        case "settop":
          return "hdd"
        case "console":
          return "controller"
        case "audio":
          return "speaker"
        case "tv":
          return "tv"
        case "remote":
          return "calculator"
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
        default:
          return "hdd"
      }
    }
  }
}