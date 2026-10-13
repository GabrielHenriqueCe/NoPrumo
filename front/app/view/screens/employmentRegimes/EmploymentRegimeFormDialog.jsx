import { SelectField } from '../../ui/SelectField';
import { useState } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Dialog } from '../../ui/Dialog';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { useT } from '../../preferences/preferencesContext'

export function EmploymentRegimeFormDialog({ regime, onClose, onSave }) {
    const t = useT()
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
                setFormError(error.message || t('employmentRegimeFormDialog.anUnexpectedErrorOccurred'));
            }
        }
    };

    return (
        <Dialog open={true} title={regime ? t('employmentRegimeFormDialog.editRegime') : t('employmentRegimeFormDialog.newRegime')} onClose={onClose}>
            <form onSubmit={handleSubmit} className="flex flex-col gap-4 p-6">
                {formError && <div className="p-3 bg-danger-wash text-danger rounded text-sm">{formError}</div>}
                <TextField
                    label={t('employmentRegimeFormDialog.labelEGCltPj')}
                    value={label}
                    onChange={e => setLabel(e.target.value)}
                    error={fieldErrors.label}
                />

                <SelectField
                    label={t('employmentRegimeFormDialog.unit')}
                    value={unit}
                    onChange={e => setUnit(e.target.value)}
                    error={fieldErrors.unit}
                    options={[
                        { value: 'month', label: t('employmentRegimeFormDialog.month') },
                        { value: 'hour', label: t('employmentRegimeFormDialog.hour') },
                        { value: 'day', label: t('employmentRegimeFormDialog.day') }
                    ]}
                />

                <TextField
                    label={t('employmentRegimeFormDialog.monthlyHoursOptional')}
                    type="number"
                    step="0.01"
                    value={monthlyHours}
                    onChange={e => setMonthlyHours(e.target.value)}
                    error={fieldErrors.monthlyHours}
                />

                <div className="flex flex-col gap-1">
                    <label className="text-sm font-semibold">{t('employmentRegimeFormDialog.description')}</label>
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
                        {t('common.active')}
                    </label>
                )}

                <div className="flex gap-2 mt-4 justify-end">
                    <Button type="button" variant="outline" onClick={onClose}>{t('common.cancel')}</Button>
                    <Button type="submit">{t('common.save')}</Button>
                </div>
            </form>
        </Dialog>
    );
}