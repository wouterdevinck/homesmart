<template>
  <Shutter v-for="shutter in devices" :key="shutter.deviceId" :device="shutter" />
</template>
  
<script>
import Shutter from '../../components/app/Shutter.vue'
import { mapState } from 'vuex'
export default {
  components: { Shutter },
  computed: {
    ...mapState({
      allDevices: state => state.devices.all
    }),
    devices() {
      var roomId = this.$route.params.id
      return this.allDevices.filter(device => device.roomId == roomId)
        .filter(device => device.type == 'shutter')
    }
  },
  created () {
    this.$store.dispatch('getDevices')
  }
}
</script>
  
<style scoped>

</style>