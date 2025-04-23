import FormLabel from '@mui/material/FormLabel';
import FormControl from '@mui/material/FormControl';
import FormGroup from '@mui/material/FormGroup';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormHelperText from '@mui/material/FormHelperText';
import Switch from '@mui/material/Switch';
import { useState } from 'react';

export default function SwitchMui({
    checked = "",
    handleChange = "",
    name = "gilad",
    primaryLabel = "Assign responsibility",
    secondaryLabel = "Secondary",
    disabled = false,
    error = false,
    helperText = "Helper text",
}) {
  const [state, setState] = useState({
    gilad: true,
    jason: false,
    antoine: true,
  });

  const handleChangeDefault = (event) => {
    setState({
      ...state,
      [event.target.name]: event.target.checked,
    });
  };

  return (
    <FormControl error={error} component="fieldset" variant="standard">
      <FormLabel component="legend">{primaryLabel}</FormLabel>
      <FormGroup>
        <FormControlLabel
            sx={
                secondaryLabel === "" ? 
                {
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                }
                
                : 

                {}
}
            name={name}
            control={
                <Switch disabled={disabled} checked={ checked === "" ? state.gilad : checked} onChange={handleChange === "" ? handleChangeDefault : handleChange} name={name} />
            }
            label={secondaryLabel}
        />
      </FormGroup>
      <FormHelperText>{helperText}</FormHelperText>
    </FormControl>
  );
}