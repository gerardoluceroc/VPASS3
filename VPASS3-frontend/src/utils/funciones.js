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

/**
 * Compara si la primera fecha es menor o igual a la segunda.
 * @param {string} fecha1 - Fecha en formato 'YYYY-MM-DD'
 * @param {string} fecha2 - Fecha en formato 'YYYY-MM-DD'
 * @returns {boolean} true si fecha1 <= fecha2, false si fecha1 > fecha2
 */
export function esFechaMenorOIgual(fecha1, fecha2) {
    const d1 = dayjs(fecha1);
    const d2 = dayjs(fecha2);

    if (!d1.isValid() || !d2.isValid()) return false; // Manejo opcional

    return d1.isBefore(d2) || d1.isSame(d2);
}




