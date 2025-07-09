import * as Yup from 'yup';
import { esFechaMenorOIgual } from '../../../utils/funciones';
import { idOpcionDescargarPorRangoFechas } from './constantesDescargarCsvEncomiendas';

export const ValidationDescargarCsvEncomiendas = Yup.object().shape({
  idOpcionDescargaSeleccionada: Yup.number().required(),

  fechaInicio: Yup.string()
    .when('idOpcionDescargaSeleccionada', {
      is: idOpcionDescargarPorRangoFechas,
      then: (schema) =>
        schema
          .required('El campo es requerido')
          .test('es-fecha-valida', 'Debe ser una fecha válida', value => !!value && !isNaN(Date.parse(value)))
          .when('fechaFinal', (fechaFinal, schema) => {
            return schema.test({
              name: 'es-menor-o-igual',
              message: 'Formato incorrecto, la fecha de inicio es posterior a la fecha final',
              test: function (fechaInicio) {
                if (!fechaInicio || !fechaFinal) return true;
                return esFechaMenorOIgual(fechaInicio, fechaFinal);
              }
            });
          }),
      otherwise: (schema) => schema.notRequired().nullable()
    }),

  fechaFinal: Yup.string()
    .when('idOpcionDescargaSeleccionada', {
      is: idOpcionDescargarPorRangoFechas,
      then: (schema) =>
        schema
          .required('El campo es requerido')
          .test('es-fecha-valida', 'Debe ser una fecha válida', value => !!value && !isNaN(Date.parse(value))),
      otherwise: (schema) => schema.notRequired().nullable()
    }),
});
