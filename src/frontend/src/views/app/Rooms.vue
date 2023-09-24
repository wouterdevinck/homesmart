<template>
  <div class="card-columns">
    <Room v-for="room in rooms" :key="room.id" :room="room" @click="navigate($event, room)" />
  </div>
</template>
  
<script>
import Room from '../../components/app/Room.vue'
import { mapState } from 'vuex'
export default {
  components: { Room },
  computed: mapState({
    rooms: state => {
      let sorted = []
      let rooms = state.rooms.all
      for (let i = 0; i < rooms.length; i += 2) {
        sorted.push(rooms[i])
      }
      for (let i = 1; i < rooms.length; i += 2) {
        sorted.push(rooms[i])
      }
      return sorted
    }
  }),
  created () {
    this.$store.dispatch('getRooms')
  },
  methods: {
    navigate: function (event, room) {
      this.$router.push({ path: `/rooms/${room.id}` })
    }
  }
}
</script>
  
<style scoped>
.card-columns {
  column-count: 2;
}
</style>