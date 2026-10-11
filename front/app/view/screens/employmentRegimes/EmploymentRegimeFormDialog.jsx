import React, { useState } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Dialog } from '../../ui/Dialog';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';

export function EmploymentRegimeFormDialog({ regime, onClose, onSave }) {
    const { employmentRegimes } = useContainer();
    const [label, setLabel] = useState(regime ? regime.label : '');
    const [unit, setUnit] = useState(regime ? regime.unit : 'month');
    const [monthlyHours, setMonthlyHours] = useState(regime && regime.monthlyHours ? regime.monthlyHours : '');
    const [description, setDescription] = useState(regime && regime.description ? regime.description : '');
    const [active, setActive] = useState(regime ? regime.active : true);
    const [fieldErrors, setFieldErrors] = useState({});
    const [formError, setFormError] = useState(null);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setFieldErrors({});
        setFormError(null);
        try {
            const payload = { 
                label, 
                unit, 
                monthlyHours: monthlyHours ? Number(monthlyHours) : null,
                description,
                active 
            };

            if (regime) {
                await employmentRegimes.update(regime.id, payload);
            } else {
                await employmentRegimes.create(payload);
            }
            onSave();
        } catch (error) {
            if (error.isValidation) {
                setFieldErrors(error.fieldErrors);
            } else {
                setFormError(error.message || 'An unexpected error occurred.');
            }
        }
    };

    return (
        <Dialog open={true} title={regime ? "Edit Regime" : "New Regime"} onClose={onClose}>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                {formError && <div className="p-3 bg-red-100 text-red-700 rounded text-sm">{formError}</div>}
                <TextField 
                    label="Label (e.g. CLT, PJ)" 
                    value={label} 
                    onChange={e => setLabel(e.target.value)} 
                    error={fieldErrors.name}
                />

                <div className="flex flex-col gap-1">
                    <label className="text-sm font-semibold">Unit</label>
                    <select 
                        value={unit} 
                        onChange={e => setUnit(e.target.value)}
                        className="border border-line rounded px-3 py-2 outline-none focus:border-graphite"
                    >
                        <option value="month">Month</option>
                        <option value="hour">Hour</option>
                        <option value="day">Day</option>
                    </select>
                    {fieldErrors.unit && <span className="text-danger text-sm">{fieldErrors.unit.join(', ')}</span>}
                </div>

                <TextField 
                    label="Monthly Hours (optional)" 
                    type="number"
                    step="0.01"
                    value={monthlyHours} 
                    onChange={e => setMonthlyHours(e.target.value)} 
                    error={fieldErrors.monthlyHours}
                />

                <div className="flex flex-col gap-1">
                    <label className="text-sm font-semibold">Description</label>
                    <textarea 
                        className="border border-line rounded px-3 py-2 outline-none focus:border-graphite"
                        rows={3}
                        value={description}
                        onChange={e => setDescription(e.target.value)}
                    />
                </div>
                
                {regime && (
                    <label className="flex items-center gap-2">
                        <input type="checkbox" checked={active} onChange={e => setActive(e.target.checked)} />
                        Active
                    </label>
                )}
                
                <div className="flex gap-2 mt-4 justify-end">
                    <Button type="button" variant="outline" onClick={onClose}>Cancel</Button>
                    <Button type="submit">Save</Button>
                </div>
            </form>
        </Dialog>
    );
}