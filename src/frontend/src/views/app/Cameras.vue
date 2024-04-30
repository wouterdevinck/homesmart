<template>
  <Camera v-for="camera in devices.filter(x => x.type == 'camera')" :key="camera.deviceId" :device="camera" />
</template>
  
<script>
import Camera from '../../components/app/Camera.vue'
import { mapState } from 'vuex'
export default {
  components: { Camera },
  computed: {
    ...mapState({
      allDevices: state => state.devices.all
    }),
    devices() {
      var roomId = this.$route.params.id
      return this.allDevices.filter(device => device.roomId == roomId)
        .filter(device => device.type == 'camera')
    }
  },
  created () {
    this.$store.dispatch('getDevices')
  }
}
</script>
  
<style scoped>

</style>