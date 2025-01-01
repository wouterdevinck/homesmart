import { createStore } from 'vuex'
import rooms from'./modules/rooms'
import devices from'./modules/devices'
import automations from'./modules/automations'
import data from'./modules/data'

export default createStore({
  modules: {
    rooms,
    devices,
    automations,
    data
  }
})