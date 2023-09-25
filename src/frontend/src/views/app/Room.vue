<template>
  <div v-if="room">
    <div class="roomNavigation">
      <router-link to="/"><svg class="bi me-2" width="16" height="16"><use xlink:href="#arrow-left-circle-fill"/></svg></router-link> 
      <b>{{ room.name }}</b>
    </div>

    <!-- TEMP -->
    <Dashboard :roomId="room.id" />

  </div>
</template>
  
<script>
import Dashboard from '../../views/admin/Dashboard.vue'
import { mapState } from 'vuex'
export default {
  components: { Dashboard },
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
.roomNavigation {
  margin-bottom: 15px;
}
</style>