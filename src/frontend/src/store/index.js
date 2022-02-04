import { createStore } from 'vuex'
import devices from'./modules/devices'
import automations from'./modules/automations'

export default createStore({
  modules: {
    devices,
    automations
  }
})