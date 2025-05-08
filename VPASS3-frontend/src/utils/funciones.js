import dayjs from 'dayjs'
import localizedFormat from 'dayjs/plugin/localizedFormat'
import 'dayjs/locale/es' // Para español

// Activar plugin
dayjs.extend(localizedFormat)
dayjs.locale('es') // Establecer el idioma



/**
 * Cambia una fecha ISO a formato legible en español (Chile)
 * Ej: "2025-05-07T16:49:16.0044431" → "7 de mayo de 2025 16:49"
 */
export function cambiarFormatoHoraFecha(fechaISO) {
  return dayjs(fechaISO).format('LL HH:mm')
}





export const obtenerClaimsToken = (token) => {
  try {
    const payloadBase64 = token.split('.')[1];
    const payloadJson = atob(payloadBase64.replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(payloadJson);
  } catch (error) {
      return null;
  }
}




