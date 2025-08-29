import api from 'api'
// eslint-disable-next-line
import axios from 'utilities/axios'

export default {
  add (obj) {
    return axios.post(`${api.url}/api/Logs`, {
      params: obj
    })
  },

  get (id) {
    return axios.get(`${api.url}/api/Logs?mainTaskID=${id}`)
  }
}
