<template>
  <div v-if="room">
    <router-link to="/"><svg class="bi me-2" width="16" height="16"><use xlink:href="#arrow-left-circle-fill"/></svg></router-link> 
    <b>{{ room.name }}</b>
  </div>
</template>
  
<script>

import { mapState } from 'vuex'
export default {
  components: {  },
  computed: {
    ...mapState({
      rooms: state => state.rooms.all
    }),
    room() {
      var roomId = this.$route.params.id
      return this.rooms.find(room => room.id == roomId)
    }},
  created () {
    this.$store.dispatch('getRooms')
  }
}
</script>
  
<style scoped>

</style>