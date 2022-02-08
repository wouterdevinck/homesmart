import api from '../../api'

const state = {
  all: [],
  running: false
}

const mutations = {
  UPDATE_DEVICES (state, devices) {
    state.all = devices
  },
  UPDATE_DEVICE (state, device) {
    Object.assign(state.all.filter(x => x.deviceId == device.deviceId)[0], device)
  },
  SET_RUNNING (state, running) {
    state.running = running
  }
}

const actions = {
  getDevices ({ commit, state }) {
    if (!state.running) {
      commit('SET_RUNNING', true)
      api.getDevices(devices => {
        commit('UPDATE_DEVICES', devices)
      })
      api.subscribeTwinUpdates(device => {
        commit('UPDATE_DEVICE', device)
      })
    }
  }
}

const getters = {
  devices: state => state.all,
  deviceById: (state) => (id) => {
    return state.all.find(device => device.deviceId === id)
  },
  batteryDevices: state => state.all.filter(x => x.battery != undefined)
    .sort((a, b) => (a.battery > b.battery) ? 1 : ((b.battery > a.battery) ? -1 : 0))
}

const devicesModule = {
  state,
  mutations,
  actions,
  getters
}

export default devicesModule