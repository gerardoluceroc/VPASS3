import dayjs from 'dayjs'
import localizedFormat from 'dayjs/plugin/localizedFormat'
import 'dayjs/locale/es' // Para español
import { CommonAreaMode, idReservacionTipoReserva, idReservacionTipoUso } from './constantes'

// Activar plugin
dayjs.extend(localizedFormat)
dayjs.locale('es') // Establecer el idioma



/**
 * Cambia una fecha ISO a formato legible en español (Chile)
 * Ej: "2025-05-07T16:49:16.0044431" → "7 de mayo de 2025 16:49"
 */
export function cambiarFormatoHoraFecha(fechaISO) {
  if (!fechaISO) return '';
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

/*
  Función que genera un arreglo de objetos con números que van desde 'inicio' hasta 'fin'.
  Si 'inicio' es mayor que 'fin', el rango se genera en orden descendente.
 */
export function generarRango(inicio, fin) {
  const resultado = [];
  const paso = inicio <= fin ? 1 : -1;
  for (let i = inicio; paso > 0 ? i <= fin : i >= fin; i += paso) {
    resultado.push({ id: i, valor: i });
  }
  return resultado;
}

/*
  Función que formatea "horas" y "minutos" a un string con formato "HH:mm:ss".
  Si las horas o minutos son menores a 10, se les agrega un cero al inicio.
*/
export function cambiarAFormatoHoraMinutos(horas, minutos) {
  const pad = (num) => String(num).padStart(2, '0');
  return `${pad(horas)}:${pad(minutos)}:00`;
}


/*
  Función que convierte una cadena de texto con formato "HH:mm:ss" a un string legible.
  Ejemplo: "01:30:00" → "1 hora con 30 minutos"
  formatoLegibleDesdeHoraString("02:53:00"); -> "2 horas con 53 minutos"
  formatoLegibleDesdeHoraString("00:15:00"); -> "15 minutos"
  formatoLegibleDesdeHoraString("01:00:00"); -> "1 hora"
  formatoLegibleDesdeHoraString("00:00:00"); -> "0 minutos"
*/
export function formatoLegibleDesdeHoraString(horaString) {
  if (!horaString || typeof horaString !== 'string') return '';

  const [hh, mm] = horaString.split(':').map(Number);

  const partes = [];
  if (hh > 0) partes.push(`${hh} ${hh === 1 ? 'hora' : 'horas'}`);
  if (mm > 0) partes.push(`${mm} ${mm === 1 ? 'minuto' : 'minutos'}`);

  return partes.length > 0 ? partes.join(' con ') : '0 minutos';
}