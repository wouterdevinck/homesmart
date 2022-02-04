import api from '../../api'

const state = {
  all: []
}

const mutations = {
  UPDATE_AUTOMATIONS (state, payload) {
    state.all = payload
  }
}

const actions = {
  getAutomations ({ commit }) {
    api.getAutomations(automations => {
      commit('UPDATE_AUTOMATIONS', automations)
    })
  }
}

const getters = {
  automations: state => state.all
}

const automationsModule = {
  state,
  mutations,
  actions,
  getters
}

export default automationsModule