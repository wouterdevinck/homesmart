<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th>Name</th>
        <th>Manufacturer</th>
        <th>Battery</th>
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
        <td>{{ device.battery }}%</td>
      </tr>
    </tbody>
  </table>
</template>

<script>
import { mapGetters } from 'vuex'
export default {
  computed: {
    ...mapGetters({
      devices: 'batteryDevices'
    })
  },
  created () {
    this.$store.dispatch('getDevices')
  }
}
</script>