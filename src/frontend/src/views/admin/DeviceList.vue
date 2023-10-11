<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th>Name</th>
        <th>Manufacturer</th>
        <th>Model</th>
        <th>Version</th>
        <th>Online</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="device in devices" :key="device.deviceId">
        <td>
          <svg class="bi me-2" width="16" height="16">
            <use v-bind:xlink:href="'#' + getDeviceIcon(device)"/>
          </svg>
          {{ device.name }}
        </td>
        <td>{{ device.manufacturer }}</td>
        <td>{{ device.model ?? "n/a" }}</td>
        <td>{{ device.version ?? "n/a" }}</td>
        <td>
          <span :class="device.reachable ? 'online' : 'offline'" v-if="device.reachable != undefined">
            <svg class="bi me-2" width="16" height="16">
              <use xlink:href="#circle-fill"/>
            </svg>
          </span>
          <span v-else>n/a</span>
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script>
import { mapState } from 'vuex'
export default {
  computed: mapState({
    devices: state => state.devices.all
  }),
  created() {
    this.$store.dispatch('getDevices')
  }
}
</script>

<style scoped>
.online {
  color: green;
}
.offline {
  color: red;
}
</style>