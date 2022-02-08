<template>
  <table class="table table-striped">
    <thead>
      <tr>
        <th>Type</th>
        <th>Description</th>
        <th>Started</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="automation in automations" :key="automation">
        <td>{{ automation.type }}</td>
        <td>{{ automation.description }}</td>
        <td>{{ automation.started }}</td>
      </tr>
    </tbody>
  </table>
</template>

<script>
import { mapState } from 'vuex'
export default {
  computed: mapState({
    automations: state => state.automations.all
  }),
  created () {
    this.$store.dispatch('getAutomations')
  }
}
</script>