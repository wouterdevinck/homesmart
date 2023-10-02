import api from '../../api'

const state = {
  all: []
}

const mutations = {
  UPDATE_ROOMS (state, payload) {
    state.all = payload
  }
}

const actions = {
  getRooms ({ commit, state }) {
    if (state.all.length == 0) {
      api.getRooms(rooms => {
        commit('UPDATE_ROOMS', rooms)
      })
    }
  }
}

const getters = {
  rooms: state => state.all
}

const roomsModule = {
  state,
  mutations,
  actions,
  getters
}

export default roomsModule