import api from '../../api'

const state = {
  metadata: []
}

const mutations = {
  UPDATE_METADATA (state, payload) {
    state.metadata = payload
  }
}

const actions = {
  getMetadata ({ commit }) {
    api.getMetadata(metadata => {
      commit('UPDATE_METADATA', metadata)
    })
  }
}

const getters = {
  metadata: state => state.metadata
}

const dataModule = {
  state,
  mutations,
  actions,
  getters
}

export default dataModule